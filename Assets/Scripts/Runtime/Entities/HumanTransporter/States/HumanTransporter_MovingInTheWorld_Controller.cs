﻿using System.Reflection;
using JetBrains.Annotations;
using UnityEngine.UI;

namespace BFG.Runtime {
public class HumanTransporter_MovingInTheWorld_Controller {
    public enum State {
        MovingToTheCityHall,
        MovingToSegment,
    }

    public HumanTransporter_MovingInTheWorld_Controller(HumanTransporter_Controller controller) {
        _controller = controller;
    }

    public void OnEnter(
        HumanTransporter human,
        IMap map,
        IMapSize mapSize,
        Building cityHall
    ) {
        using var _ = Tracing.Scope();

        Tracing.Log("OnEnter");
        human.movingElapsed = 0;
        UpdateStates(human, map, mapSize, cityHall, null);
    }

    public void OnExit(
        HumanTransporter human,
        IMap map,
        IMapSize mapSize,
        Building cityHall
    ) {
        using var _ = Tracing.Scope();

        Tracing.Log("OnExit");
        human.stateMovingInTheWorld = null;
        human.movingTo = null;
        human.movingPath.Clear();
    }

    public void Update(
        HumanTransporter human,
        IMap map,
        IMapSize mapSize,
        Building cityHall,
        float dt
    ) {
        UpdateStates(human, map, mapSize, cityHall, human.segment);
    }

    public void OnSegmentChanged(
        HumanTransporter human,
        IMap map,
        IMapSize mapSize,
        Building cityHall,
        [CanBeNull]
        GraphSegment oldSegment
    ) {
        using var _ = Tracing.Scope();

        Tracing.Log("OnSegmentChanged");
        UpdateStates(human, map, mapSize, cityHall, oldSegment);
    }

    public void OnHumanMovedToTheNextTile(
        HumanTransporter human,
        HumanTransporterData data
    ) {
    }

    void UpdateStates(
        HumanTransporter human,
        IMap map,
        IMapSize mapSize,
        Building cityHall,
        [CanBeNull]
        GraphSegment oldSegment
    ) {
        using var _ = Tracing.Scope();

        if (human.segment != null) {
            if (
                !ReferenceEquals(oldSegment, human.segment)
                || human.stateMovingInTheWorld != State.MovingToSegment
            ) {
                Tracing.Log("Setting human.stateMovingInTheWorld = State.MovingToSegment");
                human.stateMovingInTheWorld = State.MovingToSegment;

                var center = human.segment.Graph.GetCenters()[0];
                var path = map.FindPath(human.movingTo ?? human.pos, center, true).Path;
                human.AddPath(path);
            }

            if (human.segment.Graph.Contains(human.pos)) {
                Tracing.Log(
                    "_controller.SetState(human, HumanTransporterState.MovingInsideSegment");
                _controller.SetState(human, HumanTransporterState.MovingInsideSegment);
            }
        }
        else if (human.stateMovingInTheWorld != State.MovingToTheCityHall) {
            Tracing.Log("human.stateMovingInTheWorld = State.MovingToTheCityHall");
            human.stateMovingInTheWorld = State.MovingToTheCityHall;

            var path = map.FindPath(human.movingTo ?? human.pos, cityHall.pos, true).Path;
            human.AddPath(path);
        }
    }

    readonly HumanTransporter_Controller _controller;
}
}