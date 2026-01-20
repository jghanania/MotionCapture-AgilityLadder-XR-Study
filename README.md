# XR Agility Ladder User Study

This repository contains a Unity project developed for a controlled user study investigating movement performance and subjective workload during agility ladder tasks under different visualization and embodiment conditions.

The study compares task execution in **Virtual Reality (VR)**, **Augmented Reality (AR)**, and the **real world**, while varying the visual representation of the ladder field. Motion capture data was recorded using an **OptiTrack** system, and participants completed a **NASA-TLX** questionnaire after each task.

---

## Publication

This project was developed as part of a user study published at the **IEEE VR Conference 2025**:

> S. Resch, J. -G. Hanania, V. Schwind, D. Völz and D. Sanchez-Morillo, "The Influence of Augmented and Virtual Reality Environments on Foot Positioning Success and Workload in Agility Ladder Exercises," 2025 IEEE Conference on Virtual Reality and 3D User Interfaces Abstracts and Workshops (VRW), Saint Malo, France, 2025, pp. 679-686, doi: [10.1109/VRW66409.2025.00139](https://doi.org/10.1109/VRW66409.2025.00139). 

---

## Study Overview



### Independent Variables
1. **Embodiment / Environment**
   - Virtual Reality (VR)
   - Augmented Reality (AR)
   - Real World

2. **Field Visualization**
   - Two different ladder field visualizations displayed during the task

### Dependent Measures
- Performance metrics recorded via OptiTrack
- Subjective workload assessed using NASA-TLX questionnaires

---

## Hardware & Software

### Hardware
- **Meta Quest 3** (VR / AR conditions)
- **OptiTrack Motion Capture System**
- Agility ladder setup
- Mixed Reality Lab facilities  
  *Frankfurt University of Applied Sciences*

### Software
- **Unity**: 2022.3.62f2 (LTS)
- **Meta SDK**: Version 67.0.0
- OptiTrack integration (via Motive / Unity plugin)

---

## Assets & Content

- **Characters**  
  Character models are based on assets from **Mixamo**, with minor texture adjustments.

- **Lab**  
  The lab 3d model was provided by the Mixed Reality Lab of the *Frankfurt University of Applied Sciences* 
  
- **Scenes & Logic**  
  All experimental logic and XR interaction features are implemented by me in **C#** within Unity.

---

## How to Open the Project

1. Install **Unity Hub**
2. Install **Git**
3. Add Unity Editor version **2022.3.62f2**
4. Clone this repository
5. Open the project via Unity Hub
6. Ensure Meta XR / Quest support is properly configured

---

## Reuse & Attribution

This project **may be reused for research or educational purposes only with proper attribution**.

If you use or adapt this project, please cite it appropriately and acknowledge:
- The original authors
- The Mixed Reality Lab at Frankfurt University of Applied Sciences
- Mixamo for character base assets

---

## Disclaimer

This project was developed for experimental research purposes. It is not intended as a production-ready application. Hardware-specific configurations (OptiTrack, Quest passthrough, lab setup) may require adaptation for other environments.
