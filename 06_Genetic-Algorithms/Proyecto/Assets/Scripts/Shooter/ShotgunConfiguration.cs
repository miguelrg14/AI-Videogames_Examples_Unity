using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static GeneticAlgorithm;

// Internal functionality types
public enum Selections
{
    ranking,
    tournament, // Torneo
    roulette    // Ruleta
}
public enum Crossovers
{
    real_uniform,
    real_plano,
    real_combined,
    real_arithmetic
}
public enum Mutations
{
    uniform,    // Uniforme
    swap        // Intercambio
}
public class ShotgunConfiguration : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform ShotPosition;
    [SerializeField] Transform Target;
    [Header("Prefab")]
    [SerializeField] Rigidbody ShotSpherePrefab;

    [Header("Test Configurable")]
    [SerializeField] bool beginningTest = true;
    [SerializeField] int testShotNumb = 50;
    [SerializeField] float beginningSpeed = 10f;
    public Selections selection;  // Selección
    public Crossovers crossover;  // Cruces
    public Mutations mutation;    // Mutaciones

    [Header("Actual Test")]
    public float XDegrees;  // Up & Down
    public float YDegrees;  // Left & Right
    public float Strength;

    [Header("Data Exports")]
    [SerializeField] GeneticAlgorithm Genetic;
    [SerializeField] Individual CurrentIndividual;



    bool _ready;

    void Start()
    {
        Genetic = new GeneticAlgorithm(testShotNumb, 10); // Initialize new geneticAlgorithm

        if (beginningTest == true)
        {
            Time.timeScale = beginningSpeed;
            _ready = false;
        }

        // Ended up initial data collection
        _ready = true;
    }
    void Update()
    {
        /// A MANO [Disparo al mejor dato recogido]
        // Dispara al mejor de todos los vistos en la recogida de disparos automáticos al inicio, no busca uno nuevo,
        //  por eso dispara al 1º de la lista de selección de fittest con el fitness
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CurrentIndividual = Genetic.GetFittest();
            ShooterConfigure(CurrentIndividual.xDegree, CurrentIndividual.yDegree, CurrentIndividual.strength);
            Shot();
        }

        /// AUTOMÁTICO INICIO [Recogida de datos]
        if (_ready)
        {
            CurrentIndividual = Genetic.GetNext(); // Buscar siguiente en la genética o reiniciarla

            if (CurrentIndividual != null)
            {
                ShooterConfigure(CurrentIndividual.xDegree, CurrentIndividual.yDegree, CurrentIndividual.strength);
                Shot();
            }
            // Last ball from last population was shot
            else
            {
                CurrentIndividual = Genetic.GetFittest();
                _ready = false;
                Time.timeScale = 1f;
            }
        }
    }

    /// <summary>
    ///     Configuración de disparo acual
    /// </summary>
    /// <param name="xDegrees"></param>
    /// <param name="yDegrees"></param>
    /// <param name="strength"></param>
    public void ShooterConfigure(float xDegrees, float yDegrees, float strength)
    {
        XDegrees = xDegrees;
        YDegrees = yDegrees;
        Strength = strength;
    }

    public void GetResult(float data)
    {
        Debug.Log($"Result {data}");
        CurrentIndividual.fitness = data;
        _ready = true;
    }

    /// <summary>
    ///     Lógica de disparo
    /// </summary>
    public void Shot()
    {
        _ready = false;

        transform.eulerAngles = new Vector3(XDegrees, YDegrees, 0);
        var shot = Instantiate(ShotSpherePrefab, ShotPosition);
        // Valores de la bala
        shot.gameObject.GetComponent<TargetTrigger>().Target = Target;
        shot.gameObject.GetComponent<TargetTrigger>().OnHitCollider += GetResult;
        shot.isKinematic = false;
        var force = transform.up * Strength;
        shot.AddForce(force, ForceMode.Impulse);
    }
}
