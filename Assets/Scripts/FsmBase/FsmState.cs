﻿namespace FsmBase
{
    public abstract class FsmState
    {
        protected readonly Fsm Fsm;

        protected FsmState(Fsm fsm)
        {
            Fsm = fsm;
        }

        public virtual void Update() {}
        public virtual void Enter() {}
        public virtual void Exit() {}
    }
}