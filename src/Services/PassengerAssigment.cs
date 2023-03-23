// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// TandemBooking.Services.PassengerAssigment
using System;
using System.Collections.Generic;
using TandemBooking.Models;
using TandemBooking.Services;
using TandemBooking.ViewModels.Booking;

public class PassengerAssigment
{
	private class Node
	{
		public List<Edge> incoming = new List<Edge>();

		public List<Edge> outgoing = new List<Edge>();

		public string name;

		public bool visited;

		public Node(string n)
		{
			name = n;
		}
	}

	private class Edge
	{
		public Node start;

		public Node end;

		public int restCapacity = 1;

		public Edge(Node s, Node e)
		{
			start = s;
			end = e;
		}
	}

	private static bool BFS(int[,] graph, int s, int t, out int[] parent, int N)
	{
		Queue<int> q = new Queue<int>();
		bool[] visited = new bool[N];
		parent = new int[N];
		for (int j = 0; j < N; j++)
		{
			visited[j] = false;
		}
		visited[s] = true;
		q.Enqueue(s);
		parent[0] = -1;
		while (q.Count > 0)
		{
			int current = q.Dequeue();
			for (int i = 0; i < N; i++)
			{
				if (graph[current, i] > 0 && !visited[i])
				{
					q.Enqueue(i);
					visited[i] = true;
					parent[i] = current;
				}
			}
		}
		return visited[t];
	}

	public static int DistributePilots(List<PilotAvailability> pilots, List<AdditionalPassengerViewModel> passengers)
	{
		if (pilots.Count <= 0)
		{
			return 0;
		}
		int N = pilots.Count + passengers.Count + 2;
		int[,] graph = new int[N, N];
		int sourceIndex = 0;
		int sinkIndex = N - 1;
		for (int j = 0; j < pilots.Count; j++)
		{
			int pilotIndex = 1 + j;
			graph[0, pilotIndex] = 1;
			for (int i = 0; i < passengers.Count; i++)
			{
				int passengerindex = 1 + pilots.Count + i;
				if (pilots[j].Pilot.InWeightRange(passengers[i].Weight))
				{
					graph[pilotIndex, passengerindex] = 1;
					graph[passengerindex, sinkIndex] = 1;
				}
			}
		}
		int[] parent = new int[N];
		int max_flow = 0;
		while (BFS(graph, sourceIndex, sinkIndex, out parent, N))
		{
			int path_flow = 1000;
			for (int v2 = sinkIndex; v2 != sourceIndex; v2 = parent[v2])
			{
				int u = parent[v2];
				path_flow = Math.Min(path_flow, graph[u, v2]);
			}
			for (int v = sinkIndex; v != sourceIndex; v = parent[v])
			{
				int u2 = parent[v];
				graph[u2, v] -= path_flow;
				graph[v, u2] += path_flow;
			}
			max_flow += path_flow;
		}
		return max_flow;
	}
}
