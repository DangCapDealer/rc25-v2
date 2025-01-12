using System.IO;
using UnityEngine;

public static class RuntimeStorageData
{
    public enum DATATYPE
    {
        NULL,
        SOUND,
        PLAYER
    }

    public enum StatusGame
    {
        Playing,
        Pause
    }

    public static StatusGame _StatusGame = RuntimeStorageData.StatusGame.Playing;
    public static SoundSerializable Sound;
    public static PlayerSerializable Player;

    private static string _path = OptimizeComponent.GetStringOptimize(Application.persistentDataPath, "/");
    private static string _dataSound = OptimizeComponent.GetStringOptimize(Application.persistentDataPath, "/", HashLib.GetHashStringAndDeviceID(StaticVariable.DATA_SOUND));
    private static string _dataPlayer = OptimizeComponent.GetStringOptimize(Application.persistentDataPath, "/", HashLib.GetHashStringAndDeviceID(StaticVariable.DATA_PLAYER));

    public static bool IsReady
    {
        get
        {
            if (Sound == null || Player == null)
                return false;
            return true;
        }
    }

    public static void ReadData()
    {
        Sound = ReadData<SoundSerializable>(DATATYPE.SOUND) as SoundSerializable;
        Player = ReadData<PlayerSerializable>(DATATYPE.PLAYER) as PlayerSerializable;
        LogSystem.LogSuccess("Load all data in game");
    }

    public static void CreateData()
    {
        Sound = ReadNew<SoundSerializable>(DATATYPE.SOUND) as SoundSerializable;
        Player = ReadNew<PlayerSerializable>(DATATYPE.PLAYER) as PlayerSerializable;
        LogSystem.LogSuccess("Load all data in game");
    }

    public static void SaveAllData()
    {
        SaveData(_dataSound, Sound);
        SaveData(_dataPlayer, Player);
        LogSystem.LogSuccess("Save all data in game");
    }

    public static T ReadData<T>(DATATYPE dataType) where T : class, new()
    {
        var dataPath = GetPath(dataType);

        if (File.Exists(dataPath))
        {
            try
            {
                var data = ReadDataExist<T>(dataPath);
                return data;
            }
            catch (System.Exception error)
            {
                var data = GetDataDefault<T>(dataType);
                return data;
            }
        }
        else
        {
            var data = GetDataDefault<T>(dataType);
            return data;
        }
    }

    public static T ReadNew<T>(DATATYPE dataType) where T : class, new()
    {
        var data = GetDataDefault<T>(dataType);
        return data;
    }

    public static void DeleteData(DATATYPE dataType)
    {
        var dataPath = GetPath(dataType);

        if (File.Exists(dataPath))
        {
            File.Delete(dataPath);
            LogSystem.LogSuccess($"Delete {dataPath} success!");
        }
        else
        {
            LogSystem.LogError($"Can't delete {dataPath} because it's not found!");
        }
    }

    public static void DeletaData()
    {
        string[] paths = System.IO.Directory.GetFiles(_path);
        for(int i = 0; i < paths.Length; i++)
        {
            File.Delete(paths[i]);
            LogSystem.LogSuccess($"Delete {paths[i]} success!");
        }
    }

    private static T GetDataDefault<T>(DATATYPE dataType) where T : class
    {
        try
        {
            switch (dataType)
            {
                case DATATYPE.SOUND:
                    var soundData = new SoundSerializable();
                    return soundData as T;
                case DATATYPE.PLAYER:
                    var playerData = new PlayerSerializable();
                    return playerData as T;
                default:
                    return null;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
        return null;
    }

    private static void SaveData<T>(string path, T data)
    {
        if (data == null) return;
        string _data = JsonUtility.ToJson(data);
        if (_data == null || _data == "" || _data == "{}") return;

        LogSystem.LogSuccess(_data);

        _data = HashLib.Base64Encode(_data);
        var encodeMD5 = HashLib.EncryptAndDeviceID(_data);
        File.WriteAllText(path, encodeMD5);
    }

    private static T ReadDataExist<T>(string path) where T : class
    {
        try
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string _data = sr.ReadToEnd();
            var decodeMD5 = HashLib.DecryptAndDeviceID(_data);
            _data = HashLib.Base64Decode(decodeMD5);

            var data = JsonUtility.FromJson<T>(_data);
            fs.Flush();
            fs.Close();
            return data;
        }
        catch (System.Exception ex)
        {

            Debug.Log(ex.Message);

        }
        return null;
    }

    private static string GetPath(DATATYPE dataType)
    {
        string dataPath = "";

        switch (dataType)
        {
            case DATATYPE.SOUND:
                dataPath = _dataSound;
                break;
            case DATATYPE.PLAYER:
                dataPath = _dataPlayer;
                break;
            default:
                break;
        }

        //LogSystem.LogSuccess(OptimizeComponent.GetStringOptimize("Load ", dataPath));

        return dataPath;
    }
}
