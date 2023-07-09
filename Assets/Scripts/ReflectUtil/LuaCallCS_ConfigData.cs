using UnityEngine;
using LuaInterface;
using System.Text;
using System.IO;
using System.IO.Compression;
using Zstandard.Net;

public static partial class LuaCallCS
{
    public static LuaTable LoadConfigData(string configName)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.streamingAssetsPath + "/ConfigData");

        LuaTable luaTable = LoadConfigData(directoryInfo, configName);

        return luaTable;
    }

    public static LuaTable LoadConfigData(DirectoryInfo directoryInfo, string configName)
    {
        LuaTable luaTable = null;

        FileInfo[] fileInfos = directoryInfo.GetFiles();

        foreach (var file in fileInfos)
        {
            string fileName = Path.GetFileNameWithoutExtension(file.FullName);

            if (fileName == configName + ".bin")
            {
                using (FileStream fileStream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    string luaTableStr = GetLuaTableStr(fileStream);
                    luaTable = LuaManager.m_luaState.DoString<LuaTable>(luaTableStr);
                }

                return luaTable;
            }
        }

        DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();

        foreach (DirectoryInfo item in directoryInfos)
        {
            luaTable = LoadConfigData(item, configName);

            if (luaTable != null)
            {
                break;
            }
        }

        return luaTable;
    }

    public static string GetLuaTableStr(FileStream fileStream)
    {
        string luaTableStr = "";

        byte[] compressData = new byte[fileStream.Length];

        fileStream.Read(compressData, 0, compressData.Length);

        using (MemoryStream inputMemoryStream = new MemoryStream(compressData))
        {
            using (ZstandardStream zstdStream = new ZstandardStream(inputMemoryStream, CompressionMode.Decompress))
            {
                using (MemoryStream outputMemoryStream = new MemoryStream())
                {
                    zstdStream.CopyTo(outputMemoryStream);
                    byte[] decompressData = outputMemoryStream.ToArray();
                    luaTableStr = Encoding.UTF8.GetString(decompressData, 0, decompressData.Length);
                }
            }
        }

        return luaTableStr;
    }
}