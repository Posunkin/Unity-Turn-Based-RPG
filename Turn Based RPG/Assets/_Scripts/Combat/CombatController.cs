using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CombatController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button _endTurnButton;

    private Queue<Character> _turnQueue = new Queue<Character>();
    private List<Character> _characters = new List<Character>();
    private Character _selectedCharacter;
    private Coroutine _characterRoutine;

    private IEnumerator Start()
    {
        _endTurnButton.onClick.AddListener(EndTurn);
        yield return new WaitForSeconds(1f);
        InitializeQueue();
        StartTurn();
    }

    #region Turn Queue
    private void InitializeQueue()
    {
        _characters.Sort(CompareInitiative);
        for (int i = 0; i < _characters.Count; i++)
        {
            _turnQueue.Enqueue(_characters[i]);
        }
        PrintQueue();
    }

    private int CompareInitiative(Character a, Character b)
    {
        return b.characterStats.initiative - a.characterStats.initiative;
    }

    public void AddCharacter(Character character)
    {
        _characters.Add(character);
    }
    #endregion

    #region Turn Flow
    private void StartTurn()
    {
        if (_turnQueue.Count > 0)
        {
            _selectedCharacter = _turnQueue.Dequeue();
            if (_characterRoutine != null) StopCoroutine(_characterRoutine);
            _characterRoutine = StartCoroutine(_selectedCharacter.StartTurn());
        }
        else 
            NextTurn();
    }

    private void EndTurn()
    {
        _selectedCharacter.EndTurn();
        StartTurn();
    }

    private void NextTurn()
    {
        for (int i = 0; i <_characters.Count; i++)
        {
            _turnQueue.Enqueue(_characters[i]);
        }
        StartTurn();
    }
    #endregion

    #region Debuging
    private void PrintQueue()
    {
        foreach (Character character in _turnQueue)
        {
            Debug.Log(character.gameObject.name + " " + character.characterStats.initiative);
        }
    }
    #endregion
}