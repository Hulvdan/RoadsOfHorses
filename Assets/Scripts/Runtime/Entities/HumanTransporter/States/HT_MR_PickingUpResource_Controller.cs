﻿using UnityEngine.Assertions;
using MRState = BFG.Runtime.HumanTransporter_MovingResource_Controller.State;

namespace BFG.Runtime {
public class HT_MR_PickingUpResource_Controller {
    public HT_MR_PickingUpResource_Controller(
        HumanTransporter_MovingResource_Controller controller
    ) {
        _controller = controller;
    }

    public void OnEnter(HumanTransporter human, HumanTransporterData data) {
        using var _ = Tracing.Scope();

        Assert.AreNotEqual(human.stateMovingResource, MRState.PickingUpResource);
        Assert.AreNotEqual(human.stateMovingResource_targetedResource, null);

        human.stateMovingResource = MRState.PickingUpResource;
        human.stateMovingResource_targetedResource.CarryingHuman = human;
    }

    public void OnExit(HumanTransporter human, HumanTransporterData data) {
        using var _ = Tracing.Scope();
        human.stateMovingResource_pickingUpResourceElapsed = 0;
        human.stateMovingResource_pickingUpResourceNormalized = 0;
    }

    public void Update(HumanTransporter human, HumanTransporterData data, float dt) {
        var res = human.stateMovingResource_targetedResource;
        Assert.AreNotEqual(res, null, "human.targetedResource != null");

        human.stateMovingResource_pickingUpResourceElapsed += dt;
        human.stateMovingResource_pickingUpResourceNormalized =
            human.stateMovingResource_pickingUpResourceElapsed / data.PickingUpResourceDuration;

        if (human.stateMovingResource_pickingUpResourceNormalized < 1) {
            return;
        }

        human.stateMovingResource_pickingUpResourceElapsed = data.PickingUpResourceDuration;
        human.stateMovingResource_pickingUpResourceNormalized = 1;

        if (res!.TransportationVertices.Count > 0) {
            var path = human.segment!.Graph.GetShortestPath(
                human.pos, res!.TransportationVertices[0]
            );
            human.AddPath(path);
            _controller.SetState(human, data, MRState.MovingResource);
        }
        else {
            _controller.SetState(human, data, MRState.PlacingResource);
        }
    }

    public void OnHumanCurrentSegmentChanged(
        HumanTransporter human,
        HumanTransporterData data,
        GraphSegment oldSegment
    ) {
        using var _ = Tracing.Scope();
    }

    readonly HumanTransporter_MovingResource_Controller _controller;
}
}
