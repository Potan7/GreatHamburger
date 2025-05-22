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

    public CodeLineExecutor(GameObject ui, BlockCodeExecutor ce) 
    {
        lineTextUI = ui;
        blockType = ui.GetComponent<CodeTextUI>().blockType;
        codeExecutor = ce;
    }
    public void SetNextCodeIndex(int nidx, int cnidx = -1)
    {
        nextIndex = nidx;
        conditionalNextIndex = cnidx;
    }

    public void ExecuteLine()
    {
        isCooltimeEnd = false;
        if (blockType == CodeBlockType.Move || blockType == CodeBlockType.Interact)
        {
            //isWorkEnd = false;
            isWorkEnd = true; //test
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
            codeExecutor.StartNextCode(idx);
        }
    }
    private bool isConditionFullfill()
    {
        if (blockType == CodeBlockType.For ||
            blockType == CodeBlockType.If ||
            blockType == CodeBlockType.While)
        {

            //조건 체크 필요
            return true;
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
        StartWork(currentExecutedLine.blockType);
    }
    private void StartWork(CodeBlockType type)
    {
        if (type == CodeBlockType.Move)
        {
            //cookingrobot 호출
        }
        else if (type == CodeBlockType.Interact)
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
    public void OccurMoveError()
    {
        //이동 기능 도중 버그 발생 시 호출 필요
    }
    public void OccurInteractionError() 
    {
        //상호작용 기능 도중 버그 발생 시 호출 필요
    }
}
