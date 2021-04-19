using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [SerializeField] private Transform start;

    [SerializeField] private Transform end;

    [SerializeField] private Transform content;

    [SerializeField] private float duration;

    [SerializeField] private Button backButton;

    private Sequence _sequence;

    private void Awake()
    {
        backButton.onClick.AddListener(()=>gameObject.SetActive(false));
    }

    private void OnEnable()
    {
        content.position = start.position;
        _sequence = DOTween.Sequence();
        _sequence.Append(content.DOMoveY(end.position.y, duration));
        _sequence.OnComplete(() => gameObject.SetActive(false));
        _sequence.Play();
    }

    private void OnDisable()
    {
       _sequence.Kill();
    }
    
}
