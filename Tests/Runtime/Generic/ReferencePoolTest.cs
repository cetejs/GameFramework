using GameFramework.Generic;
using NUnit.Framework;

public class ReferencePoolTest
{
    [TearDown]
    public void TearDown()
    {
        ReferencePool.ClearAll();
    }

    [Test]
    public void Test()
    {
        Data<int> data = new Data<int>();
        Data<int> data1 = ReferencePool.Get<Data<int>>();
        data1.item = 1;
        ReferencePool.Release(data1);
        Data<int> data2 = ReferencePool.Get<Data<int>>();
        Assert.Zero(data1.item);
        Assert.IsTrue(data1 == data2);
        Assert.IsFalse(data == data1);
    }
}
