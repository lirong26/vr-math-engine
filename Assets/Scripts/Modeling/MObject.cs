﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class MObject
{
    public enum MPrefabType { CUBE, SPHERE, CYLINDER, PRISM, PYRAMID, CONE, SQUARE};

    public enum MInteractMode { ALL, POINT_ONLY, EDGE_ONLY, FACE_ONLY};

    private Transform transform;

    private GameObject gameObject;

    private MMesh mesh;

    private MLinearEdge refEdge = null;

    private float refEdgeLength;

    private Matrix4x4 worldToLocalMatrix
    {
        get { return transform.worldToLocalMatrix; }
    }

    private Matrix4x4 localToWorldMatrix
    {
        get { return transform.localToWorldMatrix; }
    }

    public float scale
    {
        get { return transform.localScale.x; }
        set { transform.localScale = new Vector3(value, value, value); }
    }

    public Vector3 position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    public Vector3 rotation
    {
        get { return transform.eulerAngles; }
        set { transform.eulerAngles = value; }
    }

    // 针对导入模型的初始化
    public MObject(string path)
    {
        gameObject = new GameObject();
        mesh = new MMesh();
        using(StreamReader sr = new StreamReader(path + "/obj"))
        {
            string line = sr.ReadLine();
            while (line!=null)
            {
                string[] lineComponent = line.Split(' ');
                char type = Convert.ToChar(lineComponent[0]);
                switch (type)
                {
                    case 'p':
                        float x = (float)Convert.ToDouble(lineComponent[1]);
                        float y = (float)Convert.ToDouble(lineComponent[2]);
                        float z = (float)Convert.ToDouble(lineComponent[3]);
                        Vector3 position = new Vector3(x, y, z);
                        MPoint p = mesh.CreatePoint(position);
                        break;
                    case 'e':
                        char edgeType = Convert.ToChar(lineComponent[1]);
                        switch (edgeType)
                        {
                            case 'l':
                                int start = Convert.ToInt32(lineComponent[2]);
                                int end = Convert.ToInt32(lineComponent[3]);
                                List<MPoint> pointList = mesh.pointList;
                                MLinearEdge edge = mesh.CreateLinearEdge(pointList[start], pointList[end]);
                                break;
                            case 'c':
                                break;
                        }
                        break;
                    case 'f':
                        char faceType = Convert.ToChar(lineComponent[1]);
                        switch (faceType)
                        {
                            case 'p':
                                List<MLinearEdge> edgeList = new List<MLinearEdge>();
                                Debug.Log(lineComponent.Length);
                                for (int i = 2; i < lineComponent.Length-1; i++)
                                {
                                    int index = Convert.ToInt32(lineComponent[i]);
                                    edgeList.Add((MLinearEdge)mesh.edgeList[index]);
                                }
                                MPolygonFace face = mesh.CreatePolygonFace(edgeList);
                                break;
                        }
                        break;
                }
                line = sr.ReadLine();
            }
            sr.Close();
        }
        transform = gameObject.transform;
        transform.position = MDefinitions.DEFAULT_POSITION;
        scale = MDefinitions.DEFAULT_SCALE;
        InitRefEdge();
    }

    // 针对预制件的初始化
    public MObject(MPrefabType type)
    {
        gameObject = new GameObject();
        switch (type)
        {
            case MPrefabType.CUBE:
                mesh = MCube.GetMMesh();
                break;
            default:
                Debug.Log("Unknown prefab type: " + type);
                return;
        }
        transform = gameObject.transform;
        transform.position = MDefinitions.DEFAULT_POSITION;
        scale = MDefinitions.DEFAULT_SCALE;
        InitRefEdge();
    }

    public bool ExportObject(string path)
    {
        StringBuilder sb = new StringBuilder();
        List<MPoint> pointList = mesh.pointList;
        List<MEdge> edgeList = mesh.edgeList;
        List<MFace> faceList = mesh.faceList;
        foreach(MPoint point in pointList)
        {
            sb.Append(string.Format("p {0} {1} {2}\n", point.position.x, point.position.y, point.position.z));
        }
        foreach(MEdge edge in edgeList)
        {
            switch (edge.edgeType)
            {
                case MEdge.MEdgeType.LINEAR:
                    int start = pointList.IndexOf(((MLinearEdge)edge).start);
                    int end = pointList.IndexOf(((MLinearEdge)edge).end);
                    sb.Append(string.Format("e l {0} {1}\n", start, end));
                    break;
                case MEdge.MEdgeType.CURVE:
                    break;
            }
        }
        foreach(MFace face in faceList)
        {
            switch (face.faceType)
            {
                case MFace.MFaceType.POLYGON:
                    sb.Append("f p ");
                    foreach(MLinearEdge edge in ((MPolygonFace)face).edgeList)
                    {
                        int index = edgeList.IndexOf(edge);
                        sb.Append(string.Format("{0} ", index));
                    }
                    sb.Append("\n");
                    break;
                case MFace.MFaceType.CIRCLE:
                    break;
                case MFace.MFaceType.CONE:
                    break;
                case MFace.MFaceType.CYLINDER:
                    break;
                case MFace.MFaceType.SPHERE:
                    break;
            }
        }
        using (StreamWriter sw = new StreamWriter(path+"/obj"))
        {
            sw.Write(sb.ToString());
            sw.Flush();
            sw.Close();
        }
        return true;
    }

    public bool HitObject(Vector3 pos)
    {
        Vector3 p = worldToLocalMatrix.MultiplyPoint(pos);
        if (mesh.boundingBox.Contains(p, MDefinitions.ACTIVE_DISTANCE))
        {
            return true;
        }
        return false;
    }

    public float Response(out MEntity entity, Vector3 pos, MInteractMode mode)
    {
        Vector3 p = worldToLocalMatrix.MultiplyPoint(pos);
        float dis = float.MaxValue;
        float res = float.MaxValue;
        MPoint point;
        MEdge edge;
        MFace face;
        MEntity e = null;
        switch (mode)
        {
            case MInteractMode.ALL:
                dis = mesh.GetClosetPoint(out point, p);
                if(dis < MDefinitions.ACTIVE_DISTANCE)
                {
                    //HitPoint(point);
                    e = point;
                    res = dis;
                    break;
                }
                dis = mesh.GetClosetEdge(out edge, p);
                if(dis < MDefinitions.ACTIVE_DISTANCE)
                {
                    //HitEdge(edge);
                    e = edge;
                    res = dis;
                    break;
                }
                dis = mesh.GetClosetFace(out face, p, true, MDefinitions.ACTIVE_DISTANCE);
                if(dis < MDefinitions.ACTIVE_DISTANCE)
                {
                    //HitFace(face);
                    e = face;
                    res = dis;
                    break;
                }
                break;
            case MInteractMode.POINT_ONLY:
                dis = mesh.GetClosetPoint(out point, p);
                if (dis < MDefinitions.ACTIVE_DISTANCE)
                {
                    //HitPoint(point);
                    e = point;
                    res = dis;
                }
                break;
            case MInteractMode.EDGE_ONLY:
                dis = mesh.GetClosetEdge(out edge, p);
                if (dis < MDefinitions.ACTIVE_DISTANCE)
                {
                    //HitEdge(edge);
                    e = edge;
                    res = dis;
                }
                break;
            case MInteractMode.FACE_ONLY:
                dis = mesh.GetClosetFace(out face, p, true, MDefinitions.ACTIVE_DISTANCE / scale);
                if (dis < MDefinitions.ACTIVE_DISTANCE)
                {
                    //HitFace(face);
                    e = face;
                    res = dis;
                }
                break;
            default:
                Debug.Log("MObject: Response: unknown interact mode");
                break;
        }
        entity = e;
        return res;
    }
    
    public void Render()
    {
        mesh.Render(localToWorldMatrix);
    }

    public void Destroy()
    {
        UnityEngine.Object.Destroy(gameObject);
    }

    public void SetRefEdge(MLinearEdge edge)
    {
        if(refEdge != null)
        {
            refEdge.entityStatus = MEntity.MEntityStatus.DEFAULT;
        }
        refEdge = edge;
        if (refEdge != null) {
            refEdgeLength = refEdge.GetLength();
            refEdge.entityStatus = MEntity.MEntityStatus.SPECIAL;
        }
    }

    public float GetEdgeLength(MEdge edge)
    {
        if(refEdge != null)return edge.GetLength() / refEdgeLength;
        return edge.GetLength();
    }

    public float GetFaceSurface(MFace face)
    {
        if (refEdge != null)return face.GetSurface() / refEdgeLength / refEdgeLength;
        return face.GetSurface();
    }

	public void CreateLinearEdge(MPoint start, MPoint end){
		mesh.CreateLinearEdge (start, end);
	}

    public MRelation getEntityRelation(MEntity e1, MEntity e2)
    {
        // TODO: 根据MEntity的不同类型来生成MRelation类，对于线和面只用考虑直线和多边形面。
        // 求距离时将线段视为直线，将多边形面视为无边界的平面
        // 注意MRelation中的distance是在世界坐标系下的距离，需要根据基准边做变换，具体参考GetEdgeLength。
        return null;
    }

    private void InitRefEdge()
    {
        refEdge = mesh.GetLinearEdge();
        if (refEdge != null) {
            refEdgeLength = refEdge.GetLength();
            refEdge.entityStatus = MEntity.MEntityStatus.SPECIAL;
        }
    }

    private void HitPoint(MPoint point)
    {
        Debug.Log("hit point " + localToWorldMatrix.MultiplyPoint(point.position));
    }

    private void HitEdge(MEdge edge)
    {
        switch (edge.edgeType)
        {
            case MEdge.MEdgeType.LINEAR:
                Debug.Log("hit edge " + localToWorldMatrix.MultiplyPoint(((MLinearEdge)edge).start.position) 
                    + " " + localToWorldMatrix.MultiplyPoint(((MLinearEdge)edge).end.position));
                break;
            case MEdge.MEdgeType.CURVE:
                Debug.Log("hit edge " + localToWorldMatrix.MultiplyPoint(((MCurveEdge)edge).center.position)
                    + " " + localToWorldMatrix.MultiplyPoint(((MCurveEdge)edge).normal) + " " + ((MCurveEdge)edge).radius);
                break;
            default:
                Debug.Log("hit unknown edge of type " + edge.edgeType);
                break;
        }
    }

    private void HitFace(MFace face)
    {
        switch (face.faceType)
        {
            case MFace.MFaceType.POLYGON:
                Debug.Log("hit face " + localToWorldMatrix.MultiplyPoint(((MPolygonFace)face).edgeList[0].start.position));
                break;
            case MFace.MFaceType.CIRCLE:
            case MFace.MFaceType.CONE:
            case MFace.MFaceType.CYLINDER:
            case MFace.MFaceType.SPHERE:
            default:
                Debug.Log("hit unknown face of type " + face.faceType);
                break;
        }
    }
}
