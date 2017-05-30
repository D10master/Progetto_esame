using UnityEngine;
using UnityEngine.AI;
//using NUnit.Framework;

public class NavMeshSurfaceModifierTests
{
    NavMeshSurface surface;
    NavMeshModifier modifier;

    public void CreatePlaneWithModifier()
    {
        var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        surface = plane.AddComponent<NavMeshSurface>();
        modifier = plane.AddComponent<NavMeshModifier>();
    }

    public void DestroyPlaneWithModifier()
    {
        GameObject.DestroyImmediate(modifier.gameObject);
    }

    public void ModifierIgnoreAffectsSelf()
    {
        modifier.ignoreFromBuild = true;

        surface.BuildNavMesh();

        //Assert.IsFalse(NavMeshSurfaceTests.HasNavMeshAtOrigin());
    }

    public void ModifierIgnoreAffectsChild()
    {
        modifier.ignoreFromBuild = true;
        modifier.GetComponent<MeshRenderer>().enabled = false;

        var childPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        childPlane.transform.SetParent(modifier.transform);

        surface.BuildNavMesh();

        //Assert.IsFalse(NavMeshSurfaceTests.HasNavMeshAtOrigin());
        GameObject.DestroyImmediate(childPlane);
    }

    public void ModifierIgnoreDoesNotAffectSibling()
    {
        modifier.ignoreFromBuild = true;
        modifier.GetComponent<MeshRenderer>().enabled = false;

        var siblingPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);

        surface.BuildNavMesh();

        //Assert.IsTrue(NavMeshSurfaceTests.HasNavMeshAtOrigin());
        GameObject.DestroyImmediate(siblingPlane);
    }

    public void ModifierOverrideAreaAffectsSelf()
    {
        modifier.area = 4;
        modifier.overrideArea = true;

        surface.BuildNavMesh();

        var expectedAreaMask = 1 << 4;
        //Assert.IsTrue(NavMeshSurfaceTests.HasNavMeshAtOrigin(expectedAreaMask));
    }

    public void ModifierOverrideAreaAffectsChild()
    {
        modifier.area = 4;
        modifier.overrideArea = true;
        modifier.GetComponent<MeshRenderer>().enabled = false;

        var childPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        childPlane.transform.SetParent(modifier.transform);

        surface.BuildNavMesh();

        var expectedAreaMask = 1 << 4;
        //Assert.IsTrue(NavMeshSurfaceTests.HasNavMeshAtOrigin(expectedAreaMask));
        GameObject.DestroyImmediate(childPlane);
    }

    public void ModifierOverrideAreaDoesNotAffectSibling()
    {
        modifier.area = 4;
        modifier.overrideArea = true;
        modifier.GetComponent<MeshRenderer>().enabled = false;

        var siblingPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);

        surface.BuildNavMesh();

        var expectedAreaMask = 1;
        //Assert.IsTrue(NavMeshSurfaceTests.HasNavMeshAtOrigin(expectedAreaMask));
        GameObject.DestroyImmediate(siblingPlane);
    }
}
