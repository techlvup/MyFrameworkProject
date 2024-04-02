using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;



public class Test
{
    private static string m_rootPath = Application.streamingAssetsPath.Substring(0, Application.streamingAssetsPath.LastIndexOf("/") + 16);



    [MenuItem("GodDragonTool/测试")]
    public static void TestTool()
    {
        string path1 = m_rootPath + "/ConfigData/测试/初始内容.txt";
        string path2 = m_rootPath + "/ConfigData/测试/未处理的二进制存储.bin";
        string path3 = m_rootPath + "/ConfigData/测试/压缩的文件.bin";
        string path4 = m_rootPath + "/ConfigData/测试/压缩并加密的文件.bin";

        byte[] data;
        byte[] compressBytes;
        byte[] encryptBytes;

        using (FileStream fileStream = new FileStream(path1, FileMode.Open))
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                fileStream.CopyTo(memoryStream);
                data = memoryStream.ToArray();
                compressBytes = LuaCallCS.CompressByteData(data);
                encryptBytes = LuaCallCS.EncryptByteData(compressBytes, Encoding.UTF8.GetBytes("95gbt368426hyb13"), Encoding.UTF8.GetBytes("i8g3451h5cxmj6rf"));
            }
        }

        using (FileStream fileStream = new FileStream(path2, FileMode.Create))
        {
            using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
            {
                binaryWriter.Write(data);
            }
        }

        using (FileStream fileStream = new FileStream(path3, FileMode.Create))
        {
            using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
            {
                binaryWriter.Write(compressBytes);
            }
        }

        using (FileStream fileStream = new FileStream(path4, FileMode.Create))
        {
            using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
            {
                binaryWriter.Write(encryptBytes);
            }
        }
    }
}