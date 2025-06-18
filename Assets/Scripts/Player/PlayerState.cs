﻿public abstract class PlayerState
{
    protected PlayerController player;

    protected PlayerState(PlayerController player)
    {
        this.player = player;
    }

    public virtual void Enter() { }

    public virtual void Update() { }

    public virtual void Exit() { }
}