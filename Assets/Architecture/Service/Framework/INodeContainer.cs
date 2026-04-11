using Service.Framework.Goals;
using System.Collections.Generic;

public interface INodeContainer
{
    List<ObjectiveAction> GetChildren();
}