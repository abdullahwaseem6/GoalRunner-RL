# Goal Runner – Unity ML-Agents (Intro to RL project)

A 3D agent learns to reach a goal in a walled arena while avoiding 2 blocks that slide back and forth. Trained with PPO (Unity 2022.3, ml-agents 2.3.0-exp.3, Python mlagents 0.30.0).

## Environment
- 12x12 arena, 4 walls, a goal at a random position, and **2 moving obstacles** sliding at random speeds (1-3.5).
- **16 arenas** in one scene = 16 parallel environments.
- Built by the editor menu **GoalRunner -> Build Arenas** (Assets/Editor/ArenaBuilder.cs).

## Agent
- **Sensors:** RayPerceptionSensor3D (tags Goal, Obstacle, Wall) + 5 vector observations (local velocity x/z, goal direction x/z, goal distance).
- **Actions:** 3 continuous: forward/back, strafe, turn.

| Event | Reward |
|---|---|
| Reach goal | +1 (episode ends) |
| Hit obstacle | -0.5 (episode ends) |
| Touch wall | -0.1 |
| Moving closer to goal | +0.1 per meter |
| Every step | -1 / 1500 |

## Training runs (500k steps each)
| Run | Change | Final mean reward |
|---|---|---|
| runner_base | 256 units x 2 layers, lr 3e-4 | **~0.98 (best)** |
| runner_lr | learning rate 1e-3 | ~0.6-0.8 (unstable) |
| runner_curiosity | + curiosity reward (0.02) | ~0.23 |
| runner_big | 512 units x 3 layers | ~0.15 (learns slower) |
| runner_small | 64 units x 1 layer | -0.69 (too small) |
| runner_base_v1 | old reward (shaping 0.05, obstacle -1) | ~-1 (did not learn) |

**Lesson:** v1 never learned because the obstacle penalty was too harsh and the distance reward too weak. Doubling shaping and halving the penalty fixed it.

## TensorBoard
Screenshots are in the screenshots folder: tb_reward.png (cumulative reward), tb_reward_hist.png, tb_episode_length.png, tb_losses.png.

## How to run
    conda activate unity
    mlagents-learn config/runner_base.yaml --run-id=runner_base
    tensorboard --logdir results

To watch the trained agent: set Behavior Parameters -> Model = Assets/Models/Runner, Behavior Type = Inference Only, press Play.

## Comparing training runs
Use a unique `--run-id` for each experiment so its results stay separate. For example:

    mlagents-learn config/runner_small.yaml --run-id=runner_small_test

Press Play in Unity when the trainer is ready. Open TensorBoard with `tensorboard --logdir results` to compare cumulative reward and episode length across runs.
