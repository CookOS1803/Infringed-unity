using System.Text;
using Bonsai.Core;
using Bonsai.Core.User;

namespace Bonsai.Standard.User
{
    [BonsaiNode("Tasks/User/", "Timer")]
    public class Wait : FailableTask
    {
        [ShowAtRuntime]
        [UnityEngine.SerializeField]
        public Utility.Timer timer = new Utility.Timer();

        public override void OnEnter()
        {
            timer.Start();
        }

        protected override Status _FailableRun()
        {
            timer.Update(UnityEngine.Time.deltaTime);
            return timer.IsDone ? Status.Success : Status.Running;
        }

        public override void Description(StringBuilder builder)
        {
            builder.AppendFormat("Wait for {0:0.00}s", timer.interval);
        }
    }
}