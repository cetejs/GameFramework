using System.Collections.Generic;
using GameFramework.Generic;

namespace GameFramework.DataNodeService
{
    public interface IDataNode : IReference
    {
        string Name { get; }

        string FullName { get; }

        IDataNode Parent { get; }

        int ChildCount { get; }

        T GetData<T>() where T : Data;

        Data GetData();

        void SetData(Data data);

        bool HasChild(int index);

        bool HasChild(string name);

        IDataNode GetChild(int index);

        IDataNode GetChild(string name);

        IDataNode GetOrAddChild(string name);

        IDataNode[] GetAllChild();

        void GetAllChild(List<IDataNode> results);

        void RemoveChild(int index);

        void RemoveChild(string name);

        void Clean();

        string ToString();

        string ToDataString();
    }
}