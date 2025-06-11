using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

class CodeLineExecutor
{
    private BlockCodeExecutor codeExecutor;

    private GameObject lineTextUI;
    public CodeBlockType blockType { get; private set; }

    private bool isCooltimeEnd;
    public bool isWorkEnd;

    private int nextIndex, conditionalNextIndex;

    const float basicCooltime = 3.0f;

    public int nodeIndex { get; private set; }
    private int forRepeatCount;

    private int compareValue1, compareValue2;
    private CodeBlockType compareType;

    public CodeLineExecutor(GameObject ui, BlockCodeExecutor ce) 
    {
        lineTextUI = ui;
        blockType = ui.GetComponent<CodeTextUI>().blockTypes[0];
        codeExecutor = ce;

        InitArgs();
    }
    private void InitArgs() 
    {
        if (blockType == CodeBlockType.Interact)
        {
            nodeIndex = int.Parse(lineTextUI.GetComponent<CodeTextUI>().currentTexts[1]);
        }
        else if (blockType == CodeBlockType.For)
        {
            forRepeatCount = int.Parse(lineTextUI.GetComponent<CodeTextUI>().currentTexts[1]);
        }
        else if (blockType == CodeBlockType.If || blockType == CodeBlockType.While)
        {
            if (lineTextUI.GetComponent<CodeTextUI>().blockTypes[1] == CodeBlockType.Hand || 
                BlockCodingUIManager.instance.IsVariableTypeBlock(lineTextUI.GetComponent<CodeTextUI>().blockTypes[1]))
            {
                compareValue1 = -1;
            }
            else
            {
                compareValue1 = int.Parse(lineTextUI.GetComponent<CodeTextUI>().currentTexts[1]);
            }
            if (lineTextUI.GetComponent<CodeTextUI>().blockTypes[3] == CodeBlockType.Hand ||
                BlockCodingUIManager.instance.IsVariableTypeBlock(lineTextUI.GetComponent<CodeTextUI>().blockTypes[3]))
            {
                compareValue2 = -1;
            }
            else
            {
                compareValue2 = int.Parse(lineTextUI.GetComponent<CodeTextUI>().currentTexts[3]);
            }

            compareType = lineTextUI.GetComponent<CodeTextUI>().blockTypes[2];
        }
    }
    public void SetNextCodeIndex(int nidx, int cnidx = -1)
    {
        nextIndex = nidx;
        conditionalNextIndex = cnidx;
    }

    public void ExecuteLine()
    {
        isCooltimeEnd = false;
        isWorkEnd = false;

        FillVariable();

        lineTextUI.GetComponent<CodeTextUI>().EmphasizeRunningText(true);
        codeExecutor.StartCoroutine(BasicCoolDownRoutine());
    }
    private int GetVariableIndex(string str) 
    {
        return char.Parse(str.Substring(str.Length - 1)) - 'A';
    }
    private void FillVariable()
    {
        if (BlockCodingUIManager.instance.IsVariableTypeBlock(blockType))
        {
            int idx = GetVariableIndex(lineTextUI.GetComponent<CodeTextUI>().currentTexts[0]);
            if (lineTextUI.GetComponent<CodeTextUI>().blockTypes[1] == CodeBlockType.Hand)
            {
                int hand = codeExecutor.GetCurrentHoldingIngredientIndex();
                if (hand == -1) ErrorOccured();
                codeExecutor.SetDynamicVariable(CodeBlockType.Ingredient, idx, hand);
            }
            else
            {
                int value = -1;
                if (BlockCodingUIManager.instance.IsValueTypeBlock(lineTextUI.GetComponent<CodeTextUI>().blockTypes[1]))
                {
                    value = int.Parse(lineTextUI.GetComponent<CodeTextUI>().currentTexts[1]);
                }
                else if (BlockCodingUIManager.instance.IsVariableTypeBlock(lineTextUI.GetComponent<CodeTextUI>().blockTypes[1]))
                {
                    int content = GetVariableIndex(lineTextUI.GetComponent<CodeTextUI>().currentTexts[1]);
                    value = codeExecutor.GetDynamicVariable(lineTextUI.GetComponent<CodeTextUI>().blockTypes[1], content);
                    if (value == -1) ErrorOccured();
                }

                codeExecutor.SetDynamicVariable(blockType, idx, value);
            }
        }

        if (blockType == CodeBlockType.Interact && nodeIndex == -1)
        {
            int idx = GetVariableIndex(lineTextUI.GetComponent<CodeTextUI>().currentTexts[1]);
            nodeIndex = codeExecutor.GetDynamicVariable(lineTextUI.GetComponent<CodeTextUI>().blockTypes[1], idx);
            if (nodeIndex == -1) ErrorOccured();
        }
        else if (blockType == CodeBlockType.For && forRepeatCount == -1)
        {
            int idx = GetVariableIndex(lineTextUI.GetComponent<CodeTextUI>().currentTexts[1]);
            forRepeatCount = codeExecutor.GetDynamicVariable(lineTextUI.GetComponent<CodeTextUI>().blockTypes[1], idx);
            if (forRepeatCount == -1) ErrorOccured();
        }
        else if (blockType == CodeBlockType.If || blockType == CodeBlockType.While)
        {
            if (compareValue1 == -1)
            {
                if (lineTextUI.GetComponent<CodeTextUI>().blockTypes[1] == CodeBlockType.Hand)
                {
                    compareValue1 = codeExecutor.GetCurrentHoldingIngredientIndex();
                    if (compareValue1 == -1) ErrorOccured();
                }
                else if (BlockCodingUIManager.instance.IsVariableTypeBlock(lineTextUI.GetComponent<CodeTextUI>().blockTypes[1]))
                {
                    int idx = GetVariableIndex(lineTextUI.GetComponent<CodeTextUI>().currentTexts[1]);
                    compareValue1 = codeExecutor.GetDynamicVariable(lineTextUI.GetComponent<CodeTextUI>().blockTypes[1], idx);
                    if (compareValue1 == -1) ErrorOccured();
                }
            }
            else if (compareValue2 == -1)
            {
                if (lineTextUI.GetComponent<CodeTextUI>().blockTypes[3] == CodeBlockType.Hand)
                {
                    compareValue2 = codeExecutor.GetCurrentHoldingIngredientIndex();
                    if (compareValue2 == -1) ErrorOccured();
                }
                else if (BlockCodingUIManager.instance.IsVariableTypeBlock(lineTextUI.GetComponent<CodeTextUI>().blockTypes[3]))
                {
                    int idx = GetVariableIndex(lineTextUI.GetComponent<CodeTextUI>().currentTexts[3]);
                    compareValue2 = codeExecutor.GetDynamicVariable(lineTextUI.GetComponent<CodeTextUI>().blockTypes[3], idx);
                    if (compareValue2 == -1) ErrorOccured();
                }
            }
        }
    }
    public void EndLine()
    {
        if (isCooltimeEnd && isWorkEnd) 
        {
            lineTextUI.GetComponent<CodeTextUI>().EmphasizeRunningText(false);
            int idx = nextIndex;
            if (!isConditionFullfill())
            {
                idx = conditionalNextIndex;
                InitArgs();
            }
            else if (blockType == CodeBlockType.For)
            {
                forRepeatCount--;
            }
            codeExecutor.StartNextCode(idx);
        }
    }
    private bool isConditionFullfill()
    {
        if (blockType == CodeBlockType.For)
        {
            return (forRepeatCount > 0);
        }
        else if (blockType == CodeBlockType.If || blockType == CodeBlockType.While)
        {
            int c1 = compareValue1;
            int c2 = compareValue2;
            //if (c1 == -1) c1 = codeExecutor.GetCurrentHoldingIngredientIndex();
            //if (c2 == -1) c2 = codeExecutor.GetCurrentHoldingIngredientIndex();

            if (compareType == CodeBlockType.Same)
            {
                return c1 == c2;
            }
            else if (compareType == CodeBlockType.Greater)
            {
                return c1 > c2;
            }
            else if (compareType == CodeBlockType.Less)
            {
                return c1 < c2;
            }
        }

        return true;
    }

    IEnumerator BasicCoolDownRoutine()
    {
        yield return new WaitForSeconds(basicCooltime);
        isCooltimeEnd = true;
        EndLine();
        yield break;
    }

    public void ErrorOccured()
    {
        Debug.Log("블록코딩 런타임 오류");
        lineTextUI.GetComponent<CodeTextUI>().EmphasizeErrorText(true);
    }
    public void ResetLine()
    {
        lineTextUI.GetComponent<CodeTextUI>().ResetTextColor();
    }
}

public class BlockCodeExecutor : MonoBehaviour
{
    private List<CodeLineExecutor> lineExes;
    private CodeLineExecutor currentExecutedLine;

    private GameObject cookingRobot;
    private bool isCodeRunning = false;

    private int[] cValues = new int[BlockCodingUIManager.VAR_LIMIT_CNT];
    private int[] iValues = new int[BlockCodingUIManager.VAR_LIMIT_CNT];
    private int[] nValues = new int[BlockCodingUIManager.VAR_LIMIT_CNT];
    public void InitBlockCodeExecutor(List<GameObject> blocks, GameObject bot)
    {
        lineExes = new List<CodeLineExecutor>();
        foreach (var b in blocks)
        {
            CodeLineExecutor line = new CodeLineExecutor(b, this);

            lineExes.Add(line);
        }
        SetNextLineIndex();

        StartCode(bot);
    }
    private void SetNextLineIndex() 
    {
        Stack<Tuple<int, CodeBlockType>> stack = new();
        List<int> breaks = new();
        for (int i = 0; i < lineExes.Count; i++)
        {
            CodeBlockType type = lineExes[i].blockType;
            if (type == CodeBlockType.For ||
                type == CodeBlockType.If ||
                type == CodeBlockType.While)
            {
                stack.Push(new(i, type));
            }
            else if (type == CodeBlockType.EndFor ||
                    type == CodeBlockType.EndIf ||
                    type == CodeBlockType.EndWhile)
            {
                for (int j = 0; j < breaks.Count; j++) 
                {
                    lineExes[breaks[j]].SetNextCodeIndex(i + 1);
                }
                breaks.Clear();

                if (stack.Count == 0)
                {
                    Debug.Log("블록코딩 신텍스 오류 - 닫는 블록에 대응할 열기 블록이 없습니다. Line: " + i);
                    return;
                }
                var t = stack.Pop();
                if ((type - t.Item2) != 1)
                {
                    Debug.Log("블록코딩 신텍스 오류 - 블록 쌍의 순서가 올바르지 않습니다. Line: " + i);
                    return;
                }

                lineExes[t.Item1].SetNextCodeIndex(t.Item1 + 1, i + 1);

                if (type == CodeBlockType.EndIf)
                {
                    lineExes[i].SetNextCodeIndex(i + 1);
                }
                else
                {
                    lineExes[i].SetNextCodeIndex(t.Item1);
                }
            }
            else if (type == CodeBlockType.Break)
            {
                breaks.Add(i);
            }
            else if (type == CodeBlockType.Continue)
            {
                var t = stack.Pop();
                lineExes[i].SetNextCodeIndex(t.Item1);
                stack.Push(t);
            }
            else
            {
                lineExes[i].SetNextCodeIndex(i + 1);
            }
        }

        if (stack.Count > 0)
        {
            Debug.Log("블록코딩 신텍스 오류 - 닫히지 않은 열기 블록이 있습니다.");
            return;
        }
    }

    public void StartCode(GameObject robot)
    {
        if (isCodeRunning) return;
        isCodeRunning = true;
        cookingRobot = robot;

        robot.GetComponent<RobotController>().occurError.RemoveAllListeners();
        robot.GetComponent<RobotController>().occurError.AddListener(OccurInteractionError);

        foreach (var l in lineExes) 
        {
            l.ResetLine();
        }
        GameManager.instance.CleanPlate();
        robot.GetComponent<RobotController>().CleanHand();

        for (int i = 0; i < BlockCodingUIManager.VAR_LIMIT_CNT; i++)
        {
            cValues[i] = -1;
            iValues[i] = -1;
            nValues[i] = -1;
        }

        Debug.Log("코드 실행 시작");
        StartNextCode(0);
    }
    private void EndCode()
    {
        Debug.Log("코드 실행 종료");
        isCodeRunning = false;
        if (GameManager.instance.CheckResult()) 
        {
            PlayerMapManager.Instance.PlateSuccess();
        }
    }
    public void StartNextCode(int nextIdx) 
    {
        if (nextIdx < 0) 
        {
            Debug.Log("블록코딩 신텍스 에러 - 코드 흐름이 정상적이지 않습니다.");
            return;
        }
        if (nextIdx >= lineExes.Count) 
        {
            EndCode();
            return;
        }
        currentExecutedLine = lineExes[nextIdx];
        currentExecutedLine.ExecuteLine();
        StartCoroutine(StartWork(currentExecutedLine));
    }
    IEnumerator StartWork(CodeLineExecutor line)
    {
        if (line.blockType == CodeBlockType.Interact)
        {
            yield return cookingRobot.GetComponent<RobotController>().StartCoroutine(cookingRobot.GetComponent<RobotController>().MoveToNodeAndInteract(BlockCodingUIManager.instance.nodeList[line.nodeIndex]));
        }
        EndWork();
        yield break;
    }
    public void EndWork() 
    {
        currentExecutedLine.isWorkEnd = true;
        currentExecutedLine.EndLine();
    }
    public int GetCurrentHoldingIngredientIndex() 
    {
        return cookingRobot.GetComponent<RobotController>().WhatIsInHand();
    }
    public void OccurInteractionError() 
    {
        currentExecutedLine.ErrorOccured();
    }

    public void SetDynamicVariable(CodeBlockType type, int idx, int value)
    {
        if (idx >= BlockCodingUIManager.VAR_LIMIT_CNT || idx < 0) return;

        if (type == CodeBlockType.CVariable) cValues[idx] = value;
        else if (type == CodeBlockType.IVariable) iValues[idx] = value;
        else if (type == CodeBlockType.NVariable) nValues[idx] = value;
    }
    public int GetDynamicVariable(CodeBlockType type, int idx) 
    {
        if (idx >= BlockCodingUIManager.VAR_LIMIT_CNT || idx < 0) return -2;

        if (type == CodeBlockType.CVariable) return cValues[idx];
        else if (type == CodeBlockType.IVariable) return iValues[idx];
        else if (type == CodeBlockType.NVariable) return nValues[idx];

        return -2;
    }
}
