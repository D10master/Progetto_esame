using UnityEngine;
using UnityEngine.AI;
//using NUnit.Framework;

public class NavMeshSurfaceModifierVolumeTests
{
    NavMeshSurface surface;
    NavMeshModifierVolume modifier;

    public void CreatePlaneAndModifierVolume()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Plane);
        surface = go.AddComponent<NavMeshSurface>();

        modifier = new GameObject().AddComponent<NavMeshModifierVolume>();
    }

    public void DestroyPlaneAndModifierVolume()
    {
        GameObject.DestroyImmediate(surface.gameObject);
        GameObject.DestroyImmediate(modifier.gameObject);
    }

    public void AreaAffectsNavMeshOverlapping()
    {
        modifier.center = Vector3.zero;
        modifier.size = Vector3.one;
        modifier.area = 4;

        surface.BuildNavMesh();

        var expectedAreaMask = 1 << 4;
        //Assert..IsTrue(NavMeshSurfaceTests.HasNavMeshAtOrigin(expectedAreaMask));
    }

    public void AreaDoesNotAffectsNavMeshWhenNotOverlapping()
    {
        modifier.center = 1.1f * Vector3.right;
        modifier.size = Vector3.one;
        modifier.area = 4;

        surface.BuildNavMesh();

        var expectedAreaMask = 1;
        //Assert..IsTrue(NavMeshSurfaceTests.HasNavMeshAtOrigin(expectedAreaMask));
    }

    public void BuildUsesOnlyIncludedModifierVolume()
    {
        modifier.center = Vector3.zero;
        modifier.size = Vector3.one;
        modifier.area = 4;
        modifier.gameObject.layer = 7;

        surface.layerMask = ~(1 << 7);
        surface.BuildNavMesh();

        var expectedAreaMask = 1;
        //Assert..IsTrue(NavMeshSurfaceTests.HasNavMeshAtOrigin(expectedAreaMask));
    }
}
