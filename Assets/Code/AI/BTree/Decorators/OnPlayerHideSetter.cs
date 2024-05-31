using UnityEngine;
using Bonsai;
using Bonsai.Core;
using Infringed.Player;

namespace Infringed.AI.BTree
{
    [BonsaiNode("Decorators/Knight/")]
    public class OnPlayerHideSetter : Decorator
    {
        private VisionController _vision;
        private EnemyController _enemy;
        private PlayerController _player;

        public override void OnStart()
        {
            _vision = Actor.GetComponent<VisionController>();
            _enemy = Actor.GetComponent<EnemyController>();
        }

        public override void OnEnter()
        {
            _vision.OnPlayerInView += _OnPlayerInView;

            base.OnEnter();
        }

        public override void OnExit()
        {
            _vision.OnPlayerInView -= _OnPlayerInView;

            if (_player != null)
            {
                _player.OnHide -= _OnHide;
                _player.OnExitHideout -= _OnExitHideout;
                _player = null;
            }
        }

        public override Status Run()
        {
            return Iterator.LastChildExitStatus.GetValueOrDefault(Status.Failure);
        }

        private void _OnPlayerInView()
        {
            if (_player == null)
            {
                _player = _vision.LastNoticedPlayer.GetComponent<PlayerController>();
                _player.OnHide += _OnHide;
                _player.OnExitHideout += _OnExitHideout;
            }
        }

        private void _OnHide()
        {
            _enemy.UnhidePlayer = true;
            _enemy.LastKnownPlayerPosition = _player.transform.position;
        }

        private void _OnExitHideout()
        {
            _enemy.UnhidePlayer = false;
        }
    }
}
