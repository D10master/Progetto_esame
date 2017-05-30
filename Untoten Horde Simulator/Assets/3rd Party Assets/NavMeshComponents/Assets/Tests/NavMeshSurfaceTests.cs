using UnityEngine;
using UnityEngine.AI;
//using NUnit.Framework;
using System.Collections;

public class NavMeshSurfaceTests
{
    GameObject plane;
    NavMeshSurface surface;

    public void CreatePlaneWithSurface()
    {
        plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        surface = new GameObject().AddComponent<NavMeshSurface>();
        //Assert..IsFalse(HasNavMeshAtOrigin());
    }

    public void DestroyPlaneWithSurface()
    {
        GameObject.DestroyImmediate(plane);
        GameObject.DestroyImmediate(surface.gameObject);
        //Assert..IsFalse(HasNavMeshAtOrigin());
    }

    public void NavMeshIsAvailableAfterBuild()
    {
        surface.BuildNavMesh();
        //Assert..IsTrue(HasNavMeshAtOrigin());
    }

    public void NavMeshCanBeRemovedAndAdded()
    {
        surface.BuildNavMesh();
        //Assert..IsTrue(HasNavMeshAtOrigin());

        surface.RemoveData();
        //Assert..IsFalse(HasNavMeshAtOrigin());

        surface.AddData();
        //Assert..IsTrue(HasNavMeshAtOrigin());
    }

    public void NavMeshIsNotAvailableWhenDisabled()
    {
        surface.BuildNavMesh();

        surface.enabled = false;
        //Assert..IsFalse(HasNavMeshAtOrigin());

        surface.enabled = true;
        //Assert..IsTrue(HasNavMeshAtOrigin());
    }

    public void CanBuildWithCustomArea()
    {
        surface.defaultArea = 4;
        var expectedAreaMask = 1 << 4;

        surface.BuildNavMesh();
        //Assert..IsTrue(HasNavMeshAtOrigin(expectedAreaMask));
    }

    public void CanBuildWithCustomAgentTypeID()
    {
        surface.agentTypeID = 1234;
        surface.BuildNavMesh();

        //Assert..IsTrue(HasNavMeshAtOrigin(NavMesh.AllAreas, 1234));
    }

    public void CanBuildCollidersAndIgnoreRenderMeshes()
    {
        plane.GetComponent<MeshRenderer>().enabled = false;

        surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        surface.BuildNavMesh();
        //Assert..IsTrue(HasNavMeshAtOrigin());

        surface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
        surface.BuildNavMesh();
        //Assert..IsFalse(HasNavMeshAtOrigin());
    }

    public void CanBuildRenderMeshesAndIgnoreColliders()
    {
        plane.GetComponent<Collider>().enabled = false;

        surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        surface.BuildNavMesh();
        //Assert..IsFalse(HasNavMeshAtOrigin());

        surface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
        surface.BuildNavMesh();
        //Assert..IsTrue(HasNavMeshAtOrigin());
    }

    public void BuildIgnoresGeometryOutsideBounds()
    {
        surface.collectObjects = CollectObjects.Volume;
        surface.center = new Vector3(20, 0, 0);
        surface.size = new Vector3(10, 10, 10);

        surface.BuildNavMesh();
        //Assert..IsFalse(HasNavMeshAtOrigin());
    }

    public void BuildIgnoresGeometrySiblings()
    {
        surface.collectObjects = CollectObjects.Children;

        surface.BuildNavMesh();
        //Assert..IsFalse(HasNavMeshAtOrigin());
    }

    public void BuildUsesOnlyIncludedLayers()
    {
        plane.layer = 4;
        surface.layerMask = ~(1 << 4);

        surface.BuildNavMesh();
        //Assert..IsFalse(HasNavMeshAtOrigin());
    }

    public void DefaultSettingsMatchBuiltinSettings()
    {
        var bs = surface.GetBuildSettings();
        //Assert..AreEqual(NavMesh.GetSettingsByIndex(0), bs);
    }


    public void ActiveSurfacesContainsOnlyActiveAndEnabledSurface()
    {
        //Assert..IsTrue(NavMeshSurface.activeSurfaces.Contains(surface));
        //Assert..AreEqual(1, NavMeshSurface.activeSurfaces.Count);

        surface.enabled = false;
        //Assert..IsFalse(NavMeshSurface.activeSurfaces.Contains(surface));
        //Assert..AreEqual(0, NavMeshSurface.activeSurfaces.Count);

        surface.enabled = true;
        surface.gameObject.SetActive(false);
        //Assert..IsFalse(NavMeshSurface.activeSurfaces.Contains(surface));
        //Assert..AreEqual(0, NavMeshSurface.activeSurfaces.Count);
    }

    public IEnumerator NavMeshMovesToSurfacePositionNextFrame()
    {
        plane.transform.position = new Vector3(100, 0, 0);
        surface.transform.position = new Vector3(100, 0, 0);
        surface.BuildNavMesh();
        //Assert..IsFalse(HasNavMeshAtOrigin());

        surface.transform.position = Vector3.zero;
        //Assert..IsFalse(HasNavMeshAtOrigin());

        yield return null;

        //Assert..IsTrue(HasNavMeshAtOrigin());
    }

    public IEnumerator UpdatingAndAddingNavMesh()
    {
        var navmeshData = new NavMeshData();
        var oper = surface.UpdateNavMesh(navmeshData);
        //Assert..IsFalse(HasNavMeshAtOrigin());

        do { yield return null; } while (!oper.isDone);
        surface.RemoveData();
        surface.navMeshData = navmeshData;
        surface.AddData();

        //Assert..IsTrue(HasNavMeshAtOrigin());
    }

    public static bool HasNavMeshAtOrigin(int areaMask = NavMesh.AllAreas, int agentTypeID = 0)
    {
        var hit = new NavMeshHit();
        var filter = new NavMeshQueryFilter();
        filter.areaMask = areaMask;
        filter.agentTypeID = agentTypeID;
        return NavMesh.SamplePosition(Vector3.zero, out hit, 0.1f, filter);
    }
}
