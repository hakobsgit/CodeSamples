using System;
using UniRx;
using Zenject;

namespace Controllers {
    public class QuestionsController : IInitializable, IDisposable {
        public ReactiveProperty<bool> Requesting { get; } = new ReactiveProperty<bool>();

        public void Toggle() {
            if (Requesting.Value) {
                Cancel();
            }
            else {
                Request();
            }
        }

        public void Request() {
            Events.ChangeQuestionRequestState(true);
        }

        public void Cancel() {
            Events.ChangeQuestionRequestState(false);
        }
        
        private void StateChanged(bool state) {
            Requesting.Value = state;
        }
        
        
        public void Initialize() {
            Events.OnQuestionRequestStateChanged += StateChanged;
        }
        
        public void Dispose() {
            Events.OnQuestionRequestStateChanged -= StateChanged;
        }
    }
}