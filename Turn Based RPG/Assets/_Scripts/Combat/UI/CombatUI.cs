using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private CharacterInitiative _characterInitiativePrefab;
    [SerializeField] private Transform _initiativeLine;
    public Button endTurnButton { get; }

    private CharacterInitiative[] _characterInitiatives;
    private int _index;

    public void InitUI(Queue<Character> characters)
    {
        _endTurnButton.onClick.AddListener(ChangeCharacter);
        _characterInitiatives = new CharacterInitiative[characters.Count];
        _index = 0;

        foreach (Character character in characters)
        {
            CharacterInitiative charInit = Instantiate(_characterInitiativePrefab, _initiativeLine);
            charInit.Set(character.portrait, character.characterName, character.fraction);
            _characterInitiatives[_index] = charInit;
            _index++;
        }
        _index = 0;
        _characterInitiatives[_index].TurnStart();
    }

    private void ChangeCharacter()
    {
        _characterInitiatives[_index].TurnEnd();
        _index = _index < _characterInitiatives.Length - 1 ? ++_index : _index = 0;
        _characterInitiatives[_index].TurnStart();
    }
}
