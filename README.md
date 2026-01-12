# Sudoku Solver (C#)

A comprehensive **C# Sudoku Solver** implementing multiple solving strategies—from **stochastic local search** to **constraint satisfaction algorithms**.  


## Implemented Solvers

### 1. Stochastic Local Search (Iterative Repair)

Treats Sudoku as an optimization problem rather than a strict constraint satisfaction task.

- Maintains **valid 3×3 subgrids** as an invariant
- Uses **hill-climbing swaps** to reduce row and column conflicts
- Escapes local minima using **random walk swaps (S-swaps)**

**Best for:**  
Understanding how local search algorithms cope with large combinatorial spaces.

---

### 2. Chronological Backtracking (CBT)

A classic **depth-first search** approach.

- Assigns values cell-by-cell
- Backtracks immediately upon constraint violation
- Tracks `nodesVisited` to measure search effort

**Best for:**  
Baseline comparison and algorithmic clarity.

---

### 3. Forward Checking (FC)

An optimized version of backtracking with early pruning.

- Before assigning a value, checks whether neighboring cells retain at least one valid option
- Detects dead-ends earlier than CBT

**Best for:**  
Demonstrating practical constraint propagation.

---

### 4. Most Constrained Variable + Forward Checking (MRV + FC)

The most efficient solver in the suite.

- Uses the **Minimum Remaining Values (MRV)** heuristic
- Selects the cell with the fewest legal moves
- Combined with forward checking for aggressive pruning

**Best for:**  
Solving **Expert-level puzzles** with minimal search effort.

---

## Algorithm Comparison

| Algorithm        | Search Strategy       | Pruning Method             | Relative Complexity |
|------------------|-----------------------|----------------------------|---------------------|
| CBT              | Depth-First Search    | None                       | Very High           |
| Forward Checking | Depth-First Search    | Domain Look-ahead          | Medium              |
| MRV + FC         | Heuristic Search      | Look-ahead + MRV           | Very Low            |
| Local Search     | Iterative Repair      | N/A (Stochastic)           | Variable            |


<img width="649" height="434" alt="image" src="https://github.com/user-attachments/assets/a6803e66-b3d5-4048-b81e-64165bf0a50e" />
<img width="971" height="379" alt="image" src="https://github.com/user-attachments/assets/54439206-6647-420c-beea-8f945de34b3e" />

Using a 17 clue sudoko these differences are highlighted the best.

---

## ⚙️ Installation & Usage

### 1. Clone the Repository

```bash
git clone https://github.com/Alek-Klem-UU/Sudoko_solver.git
cd Sudoko_solver
```

### 2. Prerequisites
.NET 6.0 SDK or higher
A C# IDE

### 3. Build and run
```bash
dotnet build
dotnet run
```

