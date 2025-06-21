using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BuffLib : MonoBehaviour
{
    public static BuffLib Instance { get; private set; }

    [SerializeField] private BuffSO[] buffs; 
    private Dictionary<string, BuffSO> buffDictionary = new Dictionary<string, BuffSO>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeBuffs();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeBuffs()
    {
        foreach (var buff in buffs)
        {
            if (buff != null && !buffDictionary.ContainsKey(buff.ID))
            {
                buffDictionary.Add(buff.ID, buff);
            }
        }
    }

    public BuffSO GetBuffByID(string id)
    {
        if (buffDictionary.TryGetValue(id, out BuffSO buff))
        {
            return buff;
        }
        return null;
    }

    public string GetRandomID()
    {
        if (buffDictionary.Count == 0)
        {
            return null; 
        }

        
        int randomIndex = UnityEngine.Random.Range(0, buffDictionary.Count);
        return buffDictionary.ElementAt(randomIndex).Key;
    }
}
