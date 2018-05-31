using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EditOperationType { CreateSentence, ChangeHitNodeTime, ChanegClickPos, PasteHitNodes, DeleteHitNodes }

public class EditOperation
{
    public EditOperationType m_Type;
    public Params m_Param;
}

public class EditorModeOperation  
{
    private List<EditOperation> m_OperationList = new List<EditOperation>();
    public void AddOperation(EditOperation operation)
    {
        m_OperationList.Add(operation);
    }

    public void UndoOperation()
    {
        if(m_OperationList.Count > 0)
        {
            EditOperation lastOp = m_OperationList[m_OperationList.Count - 1];
            switch(lastOp.m_Type)
            {
                case EditOperationType.ChanegClickPos:
                    break;
                case EditOperationType.ChangeHitNodeTime:
                    break;
                case EditOperationType.CreateSentence:
                    break;
                case EditOperationType.DeleteHitNodes:
                    break;
                case EditOperationType.PasteHitNodes:
                    break;
            }
        }
    }
}
