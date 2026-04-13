/*
 * Description: Interface that allows the custom UI editor, or other systems, to reference the Goal and ObjectiveAction classes
 */
using Service.Framework.Goals;
using System.Collections.Generic;

namespace Service.Framework
{
    public interface INodeContainer
    {
        List<ObjectiveAction> GetChildren();
    }
}