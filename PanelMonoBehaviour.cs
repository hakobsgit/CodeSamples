using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Abstraction {
    public abstract class PanelMonoBehaviour : MonoBehaviour {
        [SerializeField] protected Canvas _canvas;
        [SerializeField] protected GraphicRaycaster _graphicRaycaster;
        [SerializeField] protected CanvasGroup _canvasGroup;
        [SerializeField] protected float _fadeTime = 0.2f;
        
        public ReactiveProperty<bool> Visible = new ReactiveProperty<bool>();

        public virtual void Show(bool withAnimation = true) {
            _canvas.enabled = true;
            _graphicRaycaster.enabled = true;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.DOKill();
            _canvasGroup.DOFade(1.0f, withAnimation ? _fadeTime : 0).OnComplete(() => { Visible.Value = true; });
        }

        public virtual void Hide(bool withAnimation = true) {
            _canvasGroup.DOKill();
            _canvasGroup.DOFade(0.0f, withAnimation ? _fadeTime : 0).OnComplete(() => {
                _canvas.enabled = false;
                _graphicRaycaster.enabled = false;
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
                Visible.Value = false;
            });
        }
    }
}