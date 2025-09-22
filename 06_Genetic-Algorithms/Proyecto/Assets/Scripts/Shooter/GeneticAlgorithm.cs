using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
using Random = UnityEngine.Random;


[Serializable]
public class GeneticAlgorithm
{
    [SerializeField] ShotgunConfiguration shotConfig;

    [SerializeField] List<Individual> population;
    int _currentIndex;

    [SerializeField] int CurrentGeneration;
    [SerializeField] int MaxGenerations;

    [SerializeField] string Summary;

    public Individual ind1;
    public Individual ind2;

    public GeneticAlgorithm(int numberOfGenerations, int populationSize)
    {
        CurrentGeneration   = 0;
        MaxGenerations      = numberOfGenerations;
        GenerateRandomPopulation(populationSize);
        Summary             = "";
        shotConfig = GameObject.FindObjectOfType<ShotgunConfiguration>().gameObject.GetComponent<ShotgunConfiguration>();
    }

    void GenerateRandomPopulation(int size)
    {
        population = new List<Individual>();
        for (int i = 0; i < size; i++)
        {
            population.Add(new Individual(Random.Range(0f,90f), Random.Range(-45f, 45f), Random.Range(0f,12f)));
        }
        StartGeneration();
    }

    void StartGeneration()
    {
        _currentIndex = 0;
        CurrentGeneration ++;
    }
    public Individual GetNext()
    {
        if (_currentIndex == population.Count)
        {
            EndGeneration();
            if (CurrentGeneration >= MaxGenerations)
            {
                Debug.Log(Summary);
                return null;
            }
            StartGeneration();
        }

        return population[_currentIndex++];
    }

    /// <summary>
    ///     Se ejecuta cuando termina todos los disparos de la tanda de disparos principal
    /// </summary>
    void EndGeneration()
    {
        population.Sort();
        Summary += $"{GetFittest().fitness};";

        // Lógica de IA de Mejora
        if (CurrentGeneration < MaxGenerations)
        {
            SelectionLogic();   // Selecciono los 2 mejores padres
            CrossoverLogic();   // Los combino para sacar el mejor hijo posible
            MutationLogic();    // Cambio aleatorio para seguir buscando el mejor posible
        }
    }

    /////////// ALGORITHM LOGIC ///////////

    public Individual GetFittest()
    {
        population.Sort();
        return population[0];
    }
    public void SelectionLogic()
    {
        if (shotConfig.selection.Equals(Selections.ranking))            Selection_Ranking();            // DONE
        if (shotConfig.selection.Equals(Selections.tournament))         Selection_Tournament();         // DONE
        if (shotConfig.selection.Equals(Selections.roulette))           Selection_Roulette();           // NO
    }
    void CrossoverLogic()
    {
        if (shotConfig.crossover.Equals(Crossovers.real_uniform))       Crossover_Real_Uniform();       // DONE
        if (shotConfig.crossover.Equals(Crossovers.real_plano))         Crossover_Real_Plano();         // NO
        if (shotConfig.crossover.Equals(Crossovers.real_combined))      Crossover_Real_Combined();      // DONE
        if (shotConfig.crossover.Equals(Crossovers.real_arithmetic))    Crossover_Real_Arithmetic();    // NO
    }
    void MutationLogic()
    {
        if (shotConfig.mutation.Equals(Mutations.uniform))              Mutation_Uniform();             // DONE
        if (shotConfig.mutation.Equals(Mutations.swap))                 Mutation_Swap();                // DONE
    }

    #region Selection
    /// <summary>
    ///      Los individuos se ordenan en una lista atendiendo a su fitness.
    ///      La probabilidad de que un individuo sea elegido para el cruce es mayor cuanto mejor sea su posición en dicha lista.
    /// </summary>
    public void Selection_Ranking()
    {
        ind1 = population[0];
        ind2 = population[1];
    }
    /// <summary>
    ///     Coge 2 valores random de population y los enfrenta escogiendo al de mejor fitness de los 2
    /// </summary>
    public void Selection_Tournament()
    {
        int tournamentSize = 2; // Tournament size (you can adjust this according to your needs)
        int selectionSize = 2;  // Number of groups of individuals to select

        // First Child
        for (int i = 0; i < selectionSize; i++)
        {
            // Randomly select individuals for the tournament
            List<Individual> tournamentParticipants = new List<Individual>();
            for (int j = 0; j < tournamentSize; j++)
            {
                int randomIndex = Random.Range(0, population.Count);
                tournamentParticipants.Add(population[randomIndex]);
            }

            // Find the best individual in the tournament
            Individual winner = tournamentParticipants[0];
            foreach (Individual participant in tournamentParticipants)
            {
                if (participant.CompareTo(winner) < 0)
                {
                    winner = participant;
                }
            }

            // Assign winners to global variables to use them in crossover
            if (i == 0)
            {
                ind1 = winner;
            }
            else if (i == 1)
            {
                ind2 = winner;
            }
        }
    }
    /// <summary>
    ///      Divide una ruleta (círculo) en tantos sectores como individuos de la población.
    ///      El tamaño de cada sector es directamente proporcional al fitness del individuo al que representa.
    /// </summary>
    public void Selection_Roulette()
    {
        // Calculamos la suma total de fitness de la población
        float totalFitness = 0f;
        foreach (var individual in population)
        {
            totalFitness += individual.fitness;
        }

        // Creamos una lista de probabilidad acumulada para la selección por ruleta
        List<float> cumulativeProbabilities = new List<float>();
        float cumulativeProbability = 0f;
        foreach (var individual in population)
        {
            cumulativeProbability += individual.fitness / totalFitness;
            cumulativeProbabilities.Add(cumulativeProbability);
        }

        // Creamos una nueva población seleccionando individuos mediante la ruleta
        List<Individual> newPopulation = new List<Individual>();
        for (int i = 0; i < population.Count; i++)
        {
            float randomValue = Random.value; // Generamos un número aleatorio entre 0 y 1

            // Buscamos el individuo correspondiente al valor aleatorio en la ruleta
            for (int j = 0; j < population.Count; j++)
            {
                if (randomValue <= cumulativeProbabilities[j])
                {
                    // Añadimos una copia del individuo seleccionado a la nueva población
                    newPopulation.Add(new Individual(population[j].xDegree, population[j].yDegree, population[j].strength));
                    break;
                }
            }
        }

        // Reemplazamos la población anterior con la nueva población seleccionada
        population = newPopulation;
    }
    #endregion

    #region Crossover
    /// <summary>
    ///     Intervienen 2 progenitores y generan dos descendientes.
    ///     Funciona igual que el cruce uniforme con genotipos binarios.
    ///     No se crean valores nuevos en el fenotipo, sólo se recombinan.
    /// </summary>
    void Crossover_Real_Uniform()
    {
        //Single Point Crossover//
        var new1 = new Individual(ind1.xDegree, ind2.yDegree, ind2.strength);
        var new2 = new Individual(ind2.xDegree, ind1.yDegree, ind1.strength);

        //REPLACEMENT - Borra los últimos de la población para añadir los nuevos supuestamente mejores
        population.RemoveAt(population.Count - 1);
        population.RemoveAt(population.Count - 1);
        population.Add(new1);
        population.Add(new2);
    }
    /// <summary>
    ///     Intervienen 2 progenitores y generan n descendientes.
    ///     El valor del gen descendiente en la posición i se elige aleatoriamente entre un intervalo delimitado por los genes de los progenitores hallados en la misma posición.
    /// </summary>
    void Crossover_Real_Plano()
    {
        // Crea dos hijos
        Individual child1 = new Individual(0f, 0f, 0f);
        Individual child2 = new Individual(0f, 0f, 0f);

        // Calcula un punto de cruce aleatorio
        float crossoverPoint = Random.Range(0f, 1f);

        // Realiza el cruce por plano
        child1.xDegree = ind1.xDegree * crossoverPoint + ind2.xDegree * (1 - crossoverPoint);
        child1.yDegree = ind1.yDegree * crossoverPoint + ind2.yDegree * (1 - crossoverPoint);
        child1.strength = ind1.strength * crossoverPoint + ind2.strength * (1 - crossoverPoint);

        child2.xDegree = ind2.xDegree * crossoverPoint + ind1.xDegree * (1 - crossoverPoint);
        child2.yDegree = ind2.yDegree * crossoverPoint + ind1.yDegree * (1 - crossoverPoint);
        child2.strength = ind2.strength * crossoverPoint + ind1.strength * (1 - crossoverPoint);

        // Agrega los hijos a la población
        population.RemoveAt(population.Count - 1);
        population.RemoveAt(population.Count - 1);
        population.Add(child1);
        population.Add(child2);
    }
    /// <summary>
    ///     [BLX-α] Generalización del cruce plano. El valor del gen descendiente en la posición i se elige aleatoriamente 
    ///     entre el intervalo [Ai – α*H ; Bi + α*H], donde H = |Ai – Bi|.
    /// </summary>
    void Crossover_Real_Combined()
    {
        float alpha = 0.4f;

        int valueDegreeX1 = Mathf.RoundToInt(ind1.xDegree);
        int valueDegreeX2 = Mathf.RoundToInt(ind2.xDegree);
        int h = Mathf.Abs(valueDegreeX1 - valueDegreeX2);
        float minX = Random.Range(Mathf.Min(valueDegreeX1, valueDegreeX2) - alpha * h, Mathf.Max(valueDegreeX1, valueDegreeX2) - alpha * h);

        int valueDegreeY1 = Mathf.RoundToInt(ind1.yDegree);
        int valueDegreeY2 = Mathf.RoundToInt(ind2.yDegree);
        h = Mathf.Abs(valueDegreeY1 - valueDegreeY2);
        float minY = Random.Range(Mathf.Min(valueDegreeY1, valueDegreeY2) - alpha * h, Mathf.Max(valueDegreeY1, valueDegreeY2) - alpha * h);

        int valueStrength1 = Mathf.RoundToInt(ind1.strength);
        int valueStrength2 = Mathf.RoundToInt(ind2.strength);
        h = Mathf.Abs(valueStrength1 - valueStrength2);
        float minStrength = Random.Range(Mathf.Min(valueStrength1, valueStrength2) - alpha * h, Mathf.Max(valueStrength1, valueStrength2) - alpha * h);

        var new1 = new Individual(minX, minY, minStrength);

        // REPLACEMENT
        population.RemoveAt(population.Count - 1);
        population.Add(new1);
    }
    /// <summary>
    ///     Intervienen 2 progenitores (A y B) y generan 2 descendientes (X e Y).
    ///     El valor del gen i en el descendiente X resulta de la operación Xi = r * Ai + (1 - r) * Bi
    ///     El valor del gen i en el descendiente Y resulta de la operación Yi = r * Bi + (1 - r) * Ai
    ///     Si r = 0,5, el gen resultante es la media aritmética de los progenitores.
    /// </summary>
    void Crossover_Real_Arithmetic()
    {
        // Realiza el cruce para cada par de individuos consecutivos en la población
        for (int i = 0; i < population.Count - 1; i += 2)
        {
            // Obtiene los individuos padres
            Individual parent1 = population[i];
            Individual parent2 = population[i + 1];

            // Calcula los nuevos valores para los hijos usando cruce aritmético
            float child1_xDegree = (parent1.xDegree + parent2.xDegree) / 2f;
            float child1_yDegree = (parent1.yDegree + parent2.yDegree) / 2f;
            float child1_strength = (parent1.strength + parent2.strength) / 2f;

            float child2_xDegree = Mathf.Abs(parent1.xDegree - parent2.xDegree) / 2f;
            float child2_yDegree = Mathf.Abs(parent1.yDegree - parent2.yDegree) / 2f;
            float child2_strength = Mathf.Abs(parent1.strength - parent2.strength) / 2f;

            // Crea los hijos y los agrega a la nueva población
            Individual child1 = new Individual(child1_xDegree, child1_yDegree, child1_strength);
            Individual child2 = new Individual(child2_xDegree, child2_yDegree, child2_strength);

            population.RemoveAt(population.Count - 1);
            population.RemoveAt(population.Count - 1);
            population.Add(child1);
            population.Add(child2);
        }
    }
    #endregion

    #region Mutation

    // = Probability that an arbitrary bit in a genetic sequence will be flipped from its original state.

    /// <summary>
    ///     Se selecciona un gen aleatoriamente y se muta.
    /// </summary>
    void Mutation_Uniform()
    {
        foreach (var individual in population)
        {
            if (Random.Range(0f, 1f) < 0.02f)
            {
                individual.xDegree = Random.Range(0f, 90f);
            }
            if (Random.Range(0f, 1f) < 0.02f)
            {
                individual.yDegree = Random.Range(-45f, 45f);
            }
            if (Random.Range(0f, 1f) < 0.02f)
            {
                individual.strength = Random.Range(0f, 12f);
            }
        }
    }
    /// <summary>
    ///     Se seleccionan dos genes y se intercambian de posición.
    /// </summary>
    void Mutation_Swap()
    {
        foreach (var individual in population)
        {
            if (Random.Range(0f, 1f) < 0.02f)
            {
                individual.xDegree = individual.strength * 7.5f;
            }
            if (Random.Range(0f, 1f) < 0.02f)
            {
                individual.yDegree = individual.xDegree / 3f;
            }
            if (Random.Range(0f, 1f) < 0.02f)
            {
                individual.yDegree = -(individual.xDegree / 3f);
            }
            if (Random.Range(0f, 1f) < 0.02f)
            {
                individual.strength = individual.xDegree / 7.5f;
            }
        }
    }

    #endregion
}
