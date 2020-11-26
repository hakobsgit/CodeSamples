using Data.Models;
using Data.Responses;
using DI.Facades;
using Views;

namespace Controllers.Agenda {
    public class AgendaController {
        private readonly AgendaView _view;
        private FullAgendaData _agendas;
        private bool _initialized;
        private bool _isConference;

        public AgendaData CurrentData => _isConference ? _agendas.Conference : _agendas.PanelTalk;
        
        public AgendaController(AgendaView view) {
            _view = view;
        }

        public void Invoke() {
            if (_initialized) {
                if (!_view.Visible.Value) {
                    _view.Show();
                }

                return;
            }
            _view.SetLoadingEnable(true);
            _view.Show();
            _view.OnConferenceToggleValueChanged += b => {
                _isConference = b;
                _view.InitByData(CurrentData);
            };
            Services.AgendaService.GetAgenda(AgendaResponseReceived);
        }

        private void AgendaResponseReceived(AgendaResponse response) {
            _agendas = response.Agendas;
            _view.InitByData(CurrentData);
            _view.SetLoadingEnable(false);
            _initialized = true;
        }
    }
}