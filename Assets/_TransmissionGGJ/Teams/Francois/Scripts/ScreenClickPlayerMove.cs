﻿using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(AICharacterControl))]
public class ScreenClickPlayerMove : MonoBehaviour {
    
    AICharacterControl m_AIcontroller;

    void Start()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_AIcontroller = GetComponent<AICharacterControl>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.collider.transform.tag == "Citizen")
                {
                    m_followCitizen = true;
                    m_CitizenToFollow = hit.collider.gameObject.transform;
                    m_AIcontroller.SetTarget(m_CitizenToFollow.transform.position);
                }
                else
                {
                    m_followCitizen = false;
                    m_CitizenToFollow = null;
                    m_AIcontroller.SetTarget(hit.point);
                }
            }
        }
        else if (m_followCitizen)
        {
            Debug.Log(m_CitizenToFollow.transform.position);
            m_AIcontroller.SetTarget(m_CitizenToFollow.position);

            if (!m_navMeshAgent.pathPending)
            {
                if (m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance)
                {
                    // We are near citizen
                    CitizenAgent otherCitizen = m_CitizenToFollow.GetComponent<CitizenAgent>();

                    if (otherCitizen.CanSwitchToInteracting())
                    {
                        CitizenAgent citizenAgent = GetComponent<CitizenAgent>();
                        citizenAgent.StartConversationWith(otherCitizen, 1.1, _initiator: true);
                        otherCitizen.StartConversationWith(citizenAgent, 1.1);

                        GetComponent<CitizenManager>().m_IsInteracting.Invoke(otherCitizen);
                    }
                }
            }
        }
    }

    private Transform m_CitizenToFollow;
    private bool m_followCitizen;
    private bool m_inTransmission;
    private NavMeshAgent m_navMeshAgent;
}