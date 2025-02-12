using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class InappController : MonoBehaviour, IDetailedStoreListener
{
    public event Action<bool> OnPurchaseComplete;

    public static InappController Instance = null;
    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;

    private string currentProduct;

    public List<InappProduct> products;

    [Space, Header("DEBUG"), SerializeField]
    private bool logDebug = false;
    [SerializeField] private bool IsFreeIAP = false;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (logDebug)
            Debug.Log("[InappPurchase] initializing");
        if (m_StoreController == null)
        {
            InitializePurchasing();
            if (logDebug)
                Debug.Log("[InappPurchase] initialized");
        }
    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //add product
        foreach (InappProduct product in products)
        {
            builder.AddProduct(product.productId, product.type);
        }

        UnityPurchasing.Initialize(this, builder);

        if (logDebug)
            Debug.Log("[InappPurchase] Initialize Purchasing");
    }


    public bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public string GetProductIdByIndex(int _index)
    {
        if (products.Count <= _index) return "";
        return products[_index].productId;
    }

    public int GetProductIndexById(string _id)
    {
        for(int i = 0; i < products.Count; i++)
        {
            if (products[i].productId == _id)
                return products[i].ID;
        }

        return -1;
    }

    public string GetProductInfo(string productId)
    {
        if (IsInitialized() == false)
            return "";
        var product = m_StoreController.products.WithID(productId);
        if (product != null)
        {
            return product.metadata.localizedPriceString;
        }
        return "";
    }

    public string GetProductName(string productId)
    {
        for (int i = 0; i < products.Count; i++)
        {
            if (products[i].productId == productId)
                return products[i].productName;
        }

        return "";
    }

    public ProductType GetProductType(string _id)
    {
        for (int i = 0; i < products.Count; i++)
        {
            if (products[i].productId == _id)
                return products[i].type;
        }

        return ProductType.Consumable;
    }

    int lastTimeClickBuyInappurchase = 0;
    private bool AntiSpamClick()
    {
        if (StaticTimerHelper.CurrentTimeInSecond() - lastTimeClickBuyInappurchase > 1.0)
        {
            lastTimeClickBuyInappurchase = StaticTimerHelper.CurrentTimeInSecond();
            return false;
        }
        return true;
    }    

    public void BuyProductID(string productId, Action<bool> _OnPurchaseComplete)
    {
        if(string.IsNullOrEmpty(productId)) return;
        if (AntiSpamClick() == true) return;
        var index = GetProductIndexById(productId);
        Debug.Log($"[InappPurchase] Buy IAP: {productId} | index: {index}");
#if UNITY_EDITOR
        OnPurchaseComplete = (complete) =>
        {
            _OnPurchaseComplete?.Invoke(complete);
        };
        OnPurchaseComplete?.Invoke(true);
#else
        currentProduct = productId;
        if (IsInitialized())
        {
            //if (AdsController.Instance) AdsController.Instance.hideApp = true;
            Product product = m_StoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                OnPurchaseComplete = (complete) =>
                {
                    string price = product.metadata.localizedPriceString;
                    string currencySymbol = product.metadata.isoCurrencyCode;
                    string currencyCode = product.metadata.isoCurrencyCode;

                    string isSubscription = product.definition.type == ProductType.Subscription ? "true" : "false";

                    Debug.Log("[InappPurchase] Product price: " + price);
                    Debug.Log("[InappPurchase] Currency symbol: " + currencySymbol);
                    Debug.Log("[InappPurchase] Currency code: " + currencyCode);

                    _OnPurchaseComplete?.Invoke(complete);
                };
                if (logDebug)
                    Debug.Log(string.Format("[InappPurchase] Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                if (logDebug)
                    Debug.Log("[InappPurchase] BuyProductID: FAIL. Not purchasing product" +
                        ", either is not found or is not available for purchase");
            }
        }
        else
        {
            if (logDebug)
                Debug.Log("[InappPurchase] BuyProductID FAIL. Not initialized.");
        }
#endif
    }

    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            if (logDebug)
                Debug.Log("[InappPurchase] RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            if (logDebug)
                Debug.Log("[InappPurchase] RestorePurchases started ...");

            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result, _str) =>
            {
                if (logDebug)
                    Debug.Log("[InappPurchase] RestorePurchases continuing: " + result
                        + ". If no further messages, no purchases available to restore.");

                if (result)
                {
                    RestoreProductID();
                }
            });
        }
        else
        {
            if (logDebug)
                Debug.Log("[InappPurchase] RestorePurchases FAIL. Not supported on this platform. Current = "
                    + Application.platform);
        }
    }

    public void RestoreProductID()
    {
        if (IsInitialized())
        {
            foreach (InappProduct data in products)
            {
                Product product = m_StoreController.products.WithID(data.productId);
                if (product != null)
                {
                    if (product.hasReceipt)
                        GameEvent.OnIAPurchaseMethod(data.productId);
                }
            }
        }
        else
        {
            Debug.Log("[InappPurchase] RestoreProductID FAIL. Not initialized.");
            InitializePurchasing();
        }
    }

    public bool CheckingReceiptProductID(string productID)
    {
        Product product = m_StoreController.products.WithID(productID);
        if (product != null)
        {
            if (product.hasReceipt)
                return true;
            return false;
        }
        return true;
    }    

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        if (logDebug)
            Debug.Log("[InappPurchase] OnInitialized: PASS");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;

        Debug.Log("[InappPurchase] OnInitialized: Restorepurchase");
        foreach (InappProduct data in products)
        {
            Product product = m_StoreController.products.WithID(data.productId);
            if (product != null && product.hasReceipt)
            {
                //GameEvent.OnIAPurchaseMethod(data.productId);
            }
            Debug.Log($"[InappPurchase] {product.hasReceipt} | {data.productId}");
        }
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        if (logDebug)
            Debug.Log("[InappPurchase] OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        OnPurchaseComplete?.Invoke(currentProduct == args.purchasedProduct.definition.id);
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        if (logDebug)
            Debug.Log(string.Format("[InappPurchase] OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",
                product.definition.storeSpecificId, failureReason));
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new NotImplementedException();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class InappProduct
{
    public int ID;
    public string productId;
    public string productName;
    public ProductType type;
}