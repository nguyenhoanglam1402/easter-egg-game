using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using WebSocketSharp;

public class NPCMovement : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> eggs;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private System.Random random = new System.Random();
    [SerializeField]
    private int score = 0;
    [SerializeField]
    private string name = "";
    [SerializeField]
    private string uid = "";

    WebSocket webSocket;
    DataPlayer playerData;
    int targetIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        ConnectToServer("ws://localhost:3000");
        eggs = new List<GameObject>(GameObject.FindGameObjectsWithTag("Egg"));
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        navMeshAgent.stoppingDistance = 0;
        animator.SetBool("Grounded", true);
        SetDestination();
        

    }

	private void Update()
	{
		if(navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
		{
            animator.SetFloat("MoveSpeed", 1f);
		}
	}

	private void ConnectToServer(string host)
	{
        webSocket = new WebSocket(host);
        playerData = new DataPlayer(name, uid);
        webSocket.OnMessage += (sender, e) =>
        {
            Debug.Log("Send to server" + JsonUtility.ToJson(e.Data.ToString()));
        };
        webSocket.Connect();
    }

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Egg")
		{
            Debug.Log("I collected!");
            RefreshEggList(other);
            score += 1;
            playerData.SetScore(score);
            playerData.SetVectorState(this.transform.position);
            webSocket.Send(JsonUtility.ToJson(playerData));
            SetDestination();
        }
	}

    private void RefreshEggList(Collider other)
	{
        eggs.Remove(other.gameObject);
        Destroy(other.gameObject);
        Debug.Log("Refreshed!");
    }

	private void SetDestination()
	{
        if(eggs.Count != 0)
		{
            targetIndex = random.Next(0, eggs.Count);
            Debug.Log(eggs.Count);
            navMeshAgent.SetDestination(eggs[targetIndex].transform.position);
        }
    }
}
