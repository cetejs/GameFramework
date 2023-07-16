using System.Collections.Generic;

namespace GameFramework
{
    public interface IDataNode : IReference
    {
        string Name { get; }

        string FullName { get; }

        IDataNode Parent { get; }

        int ChildCount { get; }

        T GetData<T>() where T : GameData;

        void SetData(GameData gameData);

        bool HasChild(int index);

        bool HasChild(string name);

        IDataNode GetChild(int index);

        IDataNode GetChild(string name);

        IDataNode GetOrAddChild(string name);

        IDataNode[] GetAllChild();

        void GetAllChild(List<IDataNode> results);

        void RemoveChild(int index);

        void RemoveChild(string name);
    }
}