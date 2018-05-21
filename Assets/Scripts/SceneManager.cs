﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {
    public GameObject leftController;
    public GameObject rightController;

    private List<MObject> objects;

    private MEntity leftActiveEntity;
    private MEntity rightActiveEntity;

    private List<MEntity> selectedEntity;

    private MObject selectObject;

    private MObject.MInteractMode interactMode;

	// Use this for initialization
	void Start () {
        objects = new List<MObject>();
        selectedEntity = new List<MEntity>();
        interactMode = MObject.MInteractMode.ALL;
        AddPrefabObject(MObject.MPrefabType.CUBE);
	}
	
	// Update is called once per frame
	void Update () {
        UpdateHighlight();
	}
    
    private void AddPrefabObject(MObject.MPrefabType type)
    {
        MObject obj = new MObject(type);
        objects.Add(obj);
    }

    private void UpdateHighlight()
    {
        MEntity le = GetAvailEntity(leftController.transform.position, interactMode);
        MEntity re = GetAvailEntity(rightController.transform.position, interactMode);
        if (leftActiveEntity != null) leftActiveEntity.entityStatus = MEntity.MEntityStatus.DEFAULT;
        if (rightActiveEntity != null) rightActiveEntity.entityStatus = MEntity.MEntityStatus.DEFAULT;
        if(le == re)
        {
            if (le != null) le.entityStatus = MEntity.MEntityStatus.ACTIVE;
            leftActiveEntity = le;
            rightActiveEntity = null;
        }
        else
        {
            if (le != null) le.entityStatus = MEntity.MEntityStatus.ACTIVE;
            if (re != null) re.entityStatus = MEntity.MEntityStatus.ACTIVE;
            leftActiveEntity = le;
            rightActiveEntity = re;
        }
    }

    private MEntity GetAvailEntity(Vector3 pos, MObject.MInteractMode mode)
    {
        MEntity entity = null;
        MEntity temp;
        float dis = float.MaxValue;
        float f;
        foreach(MObject obj in objects)
        {
            if (obj.HitObject(pos))
            {
                if((f = obj.Response(out temp, pos, mode)) < dis)
                {
                    dis = f;
                    entity = temp;
                }
            }
        }
        return entity;
    }
}