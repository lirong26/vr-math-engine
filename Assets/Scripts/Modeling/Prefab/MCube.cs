﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MCube
{
    static GameObject prefab = Resources.Load<GameObject>("Prefabs/Cube");

    public static Mesh GetMesh()
    {
        Mesh mesh = prefab.GetComponent<MeshFilter>().sharedMesh;
        return Mesh.Instantiate(mesh);
    }

    public static MMesh GetMMesh()
    {
        MMesh mesh = new MMesh();
        MPoint a = mesh.CreatePoint(new Vector3(-0.5f, -0.5f, -0.5f));
        MPoint b = mesh.CreatePoint(new Vector3(-0.5f, -0.5f, 0.5f));
        MPoint c = mesh.CreatePoint(new Vector3(0.5f, -0.5f, 0.5f));
        MPoint d = mesh.CreatePoint(new Vector3(0.5f, -0.5f, -0.5f));
        MPoint e = mesh.CreatePoint(new Vector3(-0.5f, 0.5f, -0.5f));
        MPoint f = mesh.CreatePoint(new Vector3(-0.5f, 0.5f, 0.5f));
        MPoint g = mesh.CreatePoint(new Vector3(0.5f, 0.5f, 0.5f));
        MPoint h = mesh.CreatePoint(new Vector3(0.5f, 0.5f, -0.5f));
        MLinearEdge ab = mesh.CreateLinearEdge(a, b);
        MLinearEdge bc = mesh.CreateLinearEdge(b, c);
        MLinearEdge cd = mesh.CreateLinearEdge(c, d);
        MLinearEdge da = mesh.CreateLinearEdge(d, a);
        MLinearEdge ef = mesh.CreateLinearEdge(e, f);
        MLinearEdge fg = mesh.CreateLinearEdge(f, g);
        MLinearEdge gh = mesh.CreateLinearEdge(g, h);
        MLinearEdge he = mesh.CreateLinearEdge(h, e);
        MLinearEdge ae = mesh.CreateLinearEdge(a, e);
        MLinearEdge bf = mesh.CreateLinearEdge(b, f);
        MLinearEdge cg = mesh.CreateLinearEdge(c, g);
        MLinearEdge dh = mesh.CreateLinearEdge(d, h);
        List<MLinearEdge> abcd = new List<MLinearEdge>();
        abcd.Add(ab);
        abcd.Add(bc);
        abcd.Add(cd);
        abcd.Add(da);
        MPolygonFace f1 = mesh.CreatePolygonFace(abcd, false);
        List<MLinearEdge> efgh = new List<MLinearEdge>();
        efgh.Add(ef);
        efgh.Add(fg);
        efgh.Add(gh);
        efgh.Add(he);
        MPolygonFace f2 = mesh.CreatePolygonFace(efgh, false);
        List<MLinearEdge> abef = new List<MLinearEdge>();
        abef.Add(ab);
        abef.Add(ef);
        abef.Add(ae);
        abef.Add(bf);
        MPolygonFace f3 = mesh.CreatePolygonFace(abef, false);
        List<MLinearEdge> bcfg = new List<MLinearEdge>();
        bcfg.Add(bc);
        bcfg.Add(fg);
        bcfg.Add(bf);
        bcfg.Add(cg);
        MPolygonFace f4 = mesh.CreatePolygonFace(bcfg, false);
        List<MLinearEdge> cdgh = new List<MLinearEdge>();
        cdgh.Add(cd);
        cdgh.Add(gh);
        cdgh.Add(cg);
        cdgh.Add(dh);
        MPolygonFace f5 = mesh.CreatePolygonFace(cdgh, false);
        List<MLinearEdge> dahe = new List<MLinearEdge>();
        dahe.Add(da);
        dahe.Add(he);
        dahe.Add(dh);
        dahe.Add(ae);
        MPolygonFace f6 = mesh.CreatePolygonFace(dahe, false);
        return mesh;
    }

    public static AABB GetAABB()
    {
        AABB aabb = new AABB(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f));
        return aabb;
    }
}