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
    private bool isWorkEnd;

    private int nextIndex, conditionalNextIndex;

    const float basicCooltime = 3.0f;

    private int nodeIndex;
    private int forRepeatCount;

    private int compareValue1, compareValue2;
    private CodeBlockType compareType;

    public CodeLineExecutor(GameObject ui, BlockCodeExecutor ce) 
    {
        lineTextUI = ui;
        blockType = ui.GetComponent<CodeTextUI>().blockType;
        codeExecutor = ce;

        if (blockType == CodeBlockType.Interact)
        {
            nodeIndex = ui.GetComponent<CodeBlock>().currentSlot.transform.GetChild(0).GetComponent<CodeBlockSlot>().currentSlotBlock.GetComponent<CodeBlock>().selectNumber;
        }
        else if (blockType == CodeBlockType.For)
        {
            forRepeatCount = ui.GetComponent<CodeBlock>().currentSlot.transform.GetChild(0).GetComponent<CodeBlockSlot>().currentSlotBlock.GetComponent<CodeBlock>().selectNumber;
        }
        else if (blockType == CodeBlockType.If || blockType == CodeBlockType.While)
        {
            compareValue1 = ui.GetComponent<CodeBlock>().currentSlot.transform.GetChild(0).GetComponent<CodeBlockSlot>().currentSlotBlock.GetComponent<CodeBlock>().selectNumber;
            compareType = ui.GetComponent<CodeBlock>().currentSlot.transform.GetChild(1).GetComponent<CodeBlockSlot>().currentType;
            compareValue2 = ui.GetComponent<CodeBlock>().currentSlot.transform.GetChild(2).GetComponent<CodeBlockSlot>().currentSlotBlock.GetComponent<CodeBlock>().selectNumber;
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
        if (blockType == CodeBlockType.Interact)
        {
            //isWorkEnd = false;
            isWorkEnd = true; //test - 로봇이 완료하는 거 기다려야 함
        }
        else
        {
            isWorkEnd = true;
        }

        lineTextUI.GetComponent<CodeTextUI>().EmphasizeText(true);
        codeExecutor.StartCoroutine(BasicCoolDownRoutine());
    }
    public void EndLine() 
    {
        if (isCooltimeEnd && isWorkEnd) 
        {
            lineTextUI.GetComponent<CodeTextUI>().EmphasizeText(false);
            int idx = nextIndex;
            if (!isConditionFullfill())
            {
                idx = conditionalNextIndex;
            }
            else 
            {
                forRepeatCount--;
            }
            codeExecutor.StartNextCode(idx);
        }
    }
    private bool isConditionFullfill()
    {
        //조건 체크 필요
        if (blockType == CodeBlockType.For)
        {
            return (forRepeatCount > 0);
        }
        else if (blockType == CodeBlockType.If || blockType == CodeBlockType.While)
        {
            //compareValue 중 손에 든 것은 어떻게 처리? 고민 좀 해봐야 함

            if (compareType == CodeBlockType.Same) 
            {
                return compareValue1 == compareValue2;
            }
            else if (compareType == CodeBlockType.Greater)
            {
                return compareValue1 > compareValue2;
            }
            else if (compareType == CodeBlockType.Less)
            {
                return compareValue1 < compareValue2;
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
}

public class BlockCodeExecutor : MonoBehaviour
{
    private List<CodeLineExecutor> lineExes;
    private CodeLineExecutor currentExecutedLine;

    private GameObject cookingRobot;
    public void InitBlockCodeExecutor(List<GameObject> blocks)
    {
        lineExes = new List<CodeLineExecutor>();
        foreach (var b in blocks)
        {
            CodeLineExecutor line = new CodeLineExecutor(b, this);

            lineExes.Add(line);
        }
        SetNextLineIndex();

        StartCode(this.gameObject);
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
                    Debug.LogError("블록코딩 신텍스 오류 - 닫는 블록에 대응할 열기 블록이 없습니다. Line: " + i);
                    return;
                }
                var t = stack.Pop();
                if ((type - t.Item2) != 1)
                {
                    Debug.LogError("블록코딩 신텍스 오류 - 블록 쌍의 순서가 올바르지 않습니다. Line: " + i);
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
            Debug.LogError("블록코딩 신텍스 오류 - 닫히지 않은 열기 블록이 있습니다.");
            return;
        }
    }

    public void StartCode(GameObject robot)
    {
        cookingRobot = robot;

        Debug.LogError("코드 실행 시작");
        StartNextCode(0);
    }
    private void EndCode()
    {
        Debug.LogError("코드 실행 종료");
        //할게있나??
    }
    public void StartNextCode(int nextIdx) 
    {
        if (nextIdx < 0) 
        {
            Debug.LogError("블록코딩 신텍스 에러 - 코드 흐름이 정상적이지 않습니다.");
            return;
        }
        if (nextIdx >= lineExes.Count) 
        {
            EndCode();
            return;
        }
        currentExecutedLine = lineExes[nextIdx];
        currentExecutedLine.ExecuteLine();
        StartWork(currentExecutedLine);
    }
    private void StartWork(CodeLineExecutor line)
    {
        if (line.blockType == CodeBlockType.Interact)
        {
            //cookingrobot 호출
        }
        else
        {
            EndWork();
        }
    }
    public void EndWork() 
    {
        //robot에서 일이 끝나면 호출 필요
        currentExecutedLine.EndLine();
    }
    public void GetCurrentHoldingIngredient() 
    {
        //조건 체크용 기능 필요
        //robot에서 현재 든 재료 참조 필요
    }
    public void OccurInteractionError() 
    {
        //상호작용 기능 도중 버그 발생 시 호출 필요
    }
}
