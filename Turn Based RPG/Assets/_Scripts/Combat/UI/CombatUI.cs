using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private CharacterInitiative _characterInitiativePrefab;
    [SerializeField] private Healthbar _healthbarPrefab;
    [SerializeField] private Transform _healthbars;
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
            Healthbar healthbar = Instantiate(_healthbarPrefab, _healthbars);
            healthbar.Init(character.characterHealth, character.characterName);
            charInit.Set(character.portrait, character.characterName, character.fraction);
            _characterInitiatives[_index] = charInit;
            character.GetComponent<GridObject>().Construct(charInit, healthbar);
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
