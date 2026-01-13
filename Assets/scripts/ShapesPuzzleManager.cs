using System;
using UnityEngine;

public class ShapesPuzzleManager : MonoBehaviour
{
    [Header("Materiais")]
    public Material doorMaterial;
    public Material tableMaterial;

    [Header("Configuração")]
    public float targetFrequency = 1f;  // 0=quadrado, 1=triangulo, 2=circulo
    public float playerFrequency = 0f;  // Controlado pelo jogador

    [Header("Controlo")]
    public float rotationSpeed = 2f;
    public bool isSolved = false;
    [SerializeField] private Animator animator;

    void Start()
    {
        // Porta: frequência fixa
        doorMaterial.SetFloat("_Frequency", targetFrequency);
    }

    void Update()
    {
        float input = Input.GetAxisRaw("Horizontal");
        playerFrequency += input * rotationSpeed * Time.deltaTime;

        // Clamp entre 0-2
        playerFrequency = Mathf.Clamp(playerFrequency, 0f, 2f);

        // Mesa: atualiza dinamicamente
        tableMaterial.SetFloat("_Frequency", playerFrequency);

        if (Mathf.Approximately(Mathf.Round(playerFrequency), targetFrequency))
        {
            isSolved = true;
            OnPuzzleSolved();
        }
        if(!isSolved) animator.SetBool("canOpen", false);

    }

    void OnPuzzleSolved()
    {
        animator.SetBool("canOpen", true);
    }
}

