using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EnergeticsController : MonoBehaviour
{
    [SerializeField] private float m_ConnectionDistance;
    public float ConnectionDistance => m_ConnectionDistance;

    private List<EnergeticsNetwork> Networks { get; set; }

    public static Action<EnergeticsControllerEventData> OnNetworksListChanged;

    private void Awake()
    {
        Networks = new List<EnergeticsNetwork>();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        List<EnergeticsNetwork> networks = new List<EnergeticsNetwork>(Networks);
        foreach(EnergeticsNetwork network in networks)
        {
            network.UpdateNetwork(deltaTime);
        }
    }

    public void ProcessEnergeticsBuildingPlacement(GridBuildProcessEventData e)
    {
        Building building = e.building;
        IEnergetics newNode = building.GetComponent<IEnergetics>();

        //check if can connect to nearby network?
        AssignEnergeticNetworkNeighbours(newNode, building);
        
        //check if connected to 2 networks
        if (newNode.NetworkNeighbours.Count > 0)
        {
            CheckExistingNetworks(newNode);
        }
        else
        {
            //if no, create new network
            CreateNetwork(newNode);
            OnNetworksListChanged?.Invoke(new EnergeticsControllerEventData(Networks));
        }   
    }

    private void AssignEnergeticNetworkNeighbours(IEnergetics newNode, Building building)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(building.transform.position, ConnectionDistance);
        for (int i = 0; i < cols.Length; i++)
        {
            IEnergetics node;
            //if yes, add connections
            if (cols[i].TryGetComponent<IEnergetics>(out node))
            {
                node.AddNetworkNeighbour(newNode);
                newNode.AddNetworkNeighbour(node);
            }

            if (node == null)
                continue;

            EnergeticsNetwork network = Networks.Find((x) => x.Nodes.Contains(node));
            network.AddNode(newNode);
        }
    }

    private void CheckExistingNetworks(IEnergetics newNode)
    {
        List<EnergeticsNetwork> networksToRemove = new List<EnergeticsNetwork>();
        EnergeticsNetwork network = null;
        foreach (IEnergetics neighbour in newNode.NetworkNeighbours)
        {
            foreach (EnergeticsNetwork n in Networks)
            {
                if (n.Nodes.Contains(neighbour))
                {
                    if (network != null)
                    {
                        networksToRemove.Add(n);
                        network = EnergeticsNetwork.MergeNetworks(network, n);
                    }
                    else
                        network = n;
                }

            }
        }
        if (networksToRemove.Count > 0)
        {
            foreach (EnergeticsNetwork n in networksToRemove)
            {
                Networks.Remove(n);
            }
            Networks.Add(network);
        }
    }

    private void CreateNetwork(IEnergetics node)
    {
        EnergeticsNetwork network = new EnergeticsNetwork(node);

        Networks.Add(network);
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying)
        {
            foreach (EnergeticsNetwork network in Networks)
            {
                foreach (EnergyParticle p in network.Particles)
                {
                    Gizmos.DrawWireSphere(p.Position, .2f);
                }
            }
        }
    }
}
