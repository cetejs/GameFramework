using System.IO;

namespace GameFramework
{
    public interface IDataTable
    {
        void Read(BinaryReader reader);
    }
}