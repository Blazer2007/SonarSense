using UnityEngine;

public class WiresManager : MonoBehaviour
{
    public WireController[] wires;

    public void CheckAllWires()
    {
        bool allCorrect = true;

        foreach (var w in wires)
        {
            if (w.connectedPin == null)
            {
                allCorrect = false;
                break;
            }

            // Aqui metes a tua regra de “parelha correta”
            // Exemplo simples: comparar nomes
            if (w.connectedPin.name != w.name.Replace("Wire", "EndPin"))
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            Debug.Log("Puzzle concluído!");
            // Chamar lógica de completar tarefa
        }
    }
}
