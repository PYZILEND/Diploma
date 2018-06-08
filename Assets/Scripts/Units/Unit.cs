using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour {

    /// <summary>
    /// Unit type
    /// </summary>
    public UnitType type;

    //Unit's health and move points
    public int healthPoints;
    public int movePoints;

    /// <summary>
    /// Cell at which unit is positioned
    /// </summary>
    public LogicalMapCell cell;

    /// <summary>
    /// Unit's allegiance
    /// </summary>
    public Allegiance allegiance;

    //Flags for attack and being destroyed
    public bool hasAttacked;
    public bool isDestroyed;

    /// <summary>
    /// References unit's transport
    /// </summary>
    public Transport transport;
    public bool isEmbarked
    {
        get
        {
            if (transport)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// References unit's platform
    /// </summary>
    public Platform platform;
    public bool isOnPlatform
    {
        get
        {
            if (platform)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Creates unit of specified type on specified cell with said allegiance
    /// </summary>
    /// <param name="type"></param>
    /// <param name="cell"></param>
    /// <param name="allegiance"></param>
    /// <returns></returns>
    public virtual void InitializeUnit(LogicalMapCell cell, Allegiance allegiance)
    {
        GameMaster.units.Add(this);

        this.transform.SetParent(cell.transform, false);
        this.cell = cell;        
        cell.unit = this;

        this.allegiance = allegiance;
        this.healthPoints = this.type.maxHealth;
        this.movePoints = 0;
        this.hasAttacked = false;
        this.isDestroyed = false;

        this.GetComponentInChildren<MeshRenderer>().material.color = AllegianceExtentions.AllegianceToColor(allegiance);
        this.ValidatePosition();

        if (cell.isCapital &&
            cell.country.allegiance != allegiance &&
            !cell.unit)
        {
            cell.country.SwitchAllegiance(allegiance);
        }
        MakeUnitCanvas();
    }

    void MakeUnitCanvas()
    {
        if (this is Plane) return;
        GameObject unitCanvas = Instantiate(PropertiesKeeper.unitCanvas, transform, false);
        unitCanvas.transform.position = transform.position;

        Color color = AllegianceExtentions.AllegianceToColor(allegiance);
        unitCanvas.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = color;
        unitCanvas.transform.GetChild(2).GetComponentInChildren<Text>().text = (type.maxMovePoints/10).ToString();

        for (int i = 1; i < unitCanvas.transform.childCount; i++)
        {
            unitCanvas.transform.GetChild(i).GetComponent<RawImage>().color = color;
            unitCanvas.transform.GetChild(i).GetComponentInChildren<Text>().color = color;
        }

        if (this is SuperUnit)
        {
            unitCanvas.transform.GetChild(5).gameObject.SetActive(true);
        }
        else
        {
            //removing icons for second attack
            Destroy(unitCanvas.transform.GetChild(5).gameObject);
        }

        if (this is AntiAir)
        {
            //removing icons for attack, range and embark
            Destroy(unitCanvas.transform.GetChild(1).gameObject);
            Destroy(unitCanvas.transform.GetChild(3).gameObject);
            Destroy(unitCanvas.transform.GetChild(4).gameObject);
            return;
        }
        if (type.attackPower == 0)
        {
            //removing icons for attack, range
            Destroy(unitCanvas.transform.GetChild(1).gameObject);
            Destroy(unitCanvas.transform.GetChild(3).gameObject);

            Vector3 pos = unitCanvas.transform.Find("Icon2").localPosition;
            pos.x = 2.19f;
            unitCanvas.transform.Find("Icon2").localPosition = pos;
            unitCanvas.transform.Find("Icon4").GetComponentInChildren<Text>().text = type.capacity.ToString();
            return;
        }
        //removing icons for embark
        Destroy(unitCanvas.transform.GetChild(4).gameObject);
        unitCanvas.transform.Find("Icon1").GetComponentInChildren<Text>().text = type.attackPower.ToString();
        unitCanvas.transform.Find("Icon3").GetComponentInChildren<Text>().text = type.attackRange.ToString();
    }

    public void HideCanvas()
    {
        if (transform.childCount < 2) return;
        Transform unitCanvas = transform.Find("UnitCanvas(Clone)");
        if (unitCanvas == null) return;
        for (int i = 1; i < unitCanvas.childCount; i++)
        {
            unitCanvas.GetChild(i).gameObject.SetActive(false);            
        }

    }

    public void ShowCanvas()
    {
        if (transform.childCount < 2) return;
        Transform unitCanvas = transform.Find("UnitCanvas(Clone)");
        if (unitCanvas == null) return;
        unitCanvas.gameObject.SetActive(true);
        if (isEmbarked)
        {
            unitCanvas.gameObject.SetActive(false);
        }
        if (this is AntiAir)
        {
            unitCanvas.Find("Icon2").gameObject.SetActive(true);
            return;
        }

        for (int i = 1; i < unitCanvas.childCount; i++)
        {
            if ((unitCanvas.GetChild(i).name == "Icon2" || unitCanvas.GetChild(i).name == "Icon4") && isOnPlatform) continue;
            unitCanvas.GetChild(i).gameObject.SetActive(true);            
        }
    }

    public void DisplayHealth()
    {
        if (this is Plane) return;
        if (transform.childCount < 2) return;
       transform.Find("UnitCanvas(Clone)").Find("HealthBar").Find("HealthBarFill").GetComponent<Image>().fillAmount = (float) healthPoints/ type.maxHealth;
    }

    void DisplayMP()
    {
        if (transform.childCount < 2) return;
        if (this is Plane) return;
        if (type.maxMovePoints == 0) return;
        transform.Find("UnitCanvas(Clone)").Find("Icon2").GetComponentInChildren<Text>().text = (movePoints/10).ToString();
        if (movePoints == 0)
        {
            transform.Find("UnitCanvas(Clone)").Find("Icon2").GetComponent<RawImage>().color = Color.gray;
            transform.Find("UnitCanvas(Clone)").Find("Icon2").GetComponentInChildren<Text>().color = Color.gray;
        }
        else
        {
            transform.Find("UnitCanvas(Clone)").Find("Icon2").GetComponent<RawImage>().color = AllegianceExtentions.AllegianceToColor(allegiance);
            transform.Find("UnitCanvas(Clone)").Find("Icon2").GetComponentInChildren<Text>().color = AllegianceExtentions.AllegianceToColor(allegiance);
        }
    }

    void DisplayAP()
    {
        if (transform.childCount < 2) return;
        if (this is Plane) return;
        if (type.attackPower == 0) return;
        if (hasAttacked)
        {
            transform.Find("UnitCanvas(Clone)").Find("Icon1").GetComponent<RawImage>().color = Color.gray;
            transform.Find("UnitCanvas(Clone)").Find("Icon1").GetComponentInChildren<Text>().color = Color.gray;
        }
        else
        {
            transform.Find("UnitCanvas(Clone)").Find("Icon1").GetComponent<RawImage>().color = AllegianceExtentions.AllegianceToColor(allegiance);
            transform.Find("UnitCanvas(Clone)").Find("Icon1").GetComponentInChildren<Text>().color = AllegianceExtentions.AllegianceToColor(allegiance);
        }
    }

    /// <summary>
    /// Resets unit to it's default state
    /// </summary>
    public virtual void ResetUnit()
    {
        healthPoints = type.maxHealth;
        movePoints = type.maxMovePoints;
        hasAttacked = false;
        isDestroyed = false;
    }

    /// <summary>
    /// Restores unit's move points and attack status
    /// If unit was destroyed on prev turn it's removed from game
    /// </summary>
    public virtual void ChangeTurn()
    {
        movePoints = type.maxMovePoints;
        DisplayMP();
        DisplayAP();
        hasAttacked = false;
        if (isDestroyed)
        {
            DestroyLogically();
        }
    }

    /// <summary>
    /// Moves unit to specified cell
    /// </summary>
    /// <param name="destination"></param>
    public virtual void MoveToCell(LogicalMapCell destination)
    {
        List<LogicalMapCell> path = Pathfinder.SearchPath(cell, destination);
        AffectCountryes(path);                

        movePoints -= destination.distance;
        cell.unit = null;
        destination.unit = this;
        cell = destination;

        DisplayMP();
        StopAllCoroutines();
        StartCoroutine(Travel(path));        
    }

    IEnumerator Travel(List<LogicalMapCell> path)
    {
        Vector3 a, b, c = path[0].GetRelativePhysicalPosition();
        Vector3 d;

        float t = Time.deltaTime * type.travelSpeed;
        for (int i = 0; i < path.Count - 1; i++)
        {
            a = c;
            b = path[i].GetRelativePhysicalPosition();
            c = (b + path[i + 1].GetRelativePhysicalPosition()) * 0.5f;
            for(; t < 1f; t += Time.deltaTime * type.travelSpeed)
            {
                transform.position = GetBezierPoint(a, b, c, t);
                ValidatePosition();
                d = GetBezierDerivative(a, b, c, t);
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
            t -= 1f;
        }
        a = c;
        b = path[path.Count - 1].GetRelativePhysicalPosition();
        c = b;
        for (; t < 1f; t += Time.deltaTime * type.travelSpeed)
        {
            transform.position = GetBezierPoint(a, b, c, t);
            ValidatePosition();
            d = GetBezierDerivative(a, b, c, t);
            transform.localRotation = Quaternion.LookRotation(d);
            yield return null;
        }

        transform.SetParent(path[path.Count-1].transform, false);
        transform.localPosition = new Vector3(0, 0, 0);
        ValidatePosition();
        d = GetBezierDerivative(a, b, c, t);
        transform.localRotation = Quaternion.LookRotation(d);
        if (isEmbarked)
        {
            GetComponentInChildren<MeshRenderer>().enabled = false;
            transform.SetParent(transport.transform, false);
            transform.localPosition = new Vector3(0, 0, 0);
        }
        else if (platform)
        {
            transform.SetParent(platform.transform, false);
            transform.localPosition = new Vector3(0, 0, 0);
            ValidatePosition();
            
        }
    }

    Vector3 GetBezierPoint(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        float r = 1f - t;
        return r * r * a + 2f * r * t * b + t * t * c;
    }

    Vector3 GetBezierDerivative(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        return 2f * ((1f - t) * (b - a) + t * (c - b));
    }

    /// <summary>
    /// Affect countryes by this unit's movement or placement
    /// </summary>
    /// <param name="destination"></param>
    public void AffectCountryes(List<LogicalMapCell> path)
    {
        //Save unit in destination to revert changes later
        //Used when we board transport or platform
        for (int i = 0; i < path.Count - 1; i++)
        {
            LogicalMapCell start = path[i];
            LogicalMapCell destination = path[i + 1];

            Unit startUnit = null;
            if (start.unit)
            {
                startUnit = start.unit;
            }
            Unit destUnit = null;
            if (destination.unit)
            {
                destUnit = destination.unit;
            }

            //If destination cell has a country
            if (destination.country)
            {
                //If we come from another country or from neutral area (ocean, impassable)
                if (start.country == null || destination.country != start.country)
                {
                    //If this changes invaded status, then country gets invaded
                    if (!destination.country.isInvaded)
                    {
                        destination.unit = this;
                        if (destination.country.isInvaded)
                        {
                            destination.country.TriggerInvasion(allegiance);
                            if (destUnit)
                            {
                                destination.unit = destUnit;
                            }
                            else
                            {
                                destination.unit = null;
                            }
                        }
                    }
                }

                //If we moved into capital and it has different allegiance
                if (destination.isCapital &&
                    destination.country.allegiance != allegiance &&
                    !destination.unit)
                {
                    destination.country.SwitchAllegiance(allegiance);
                }
            }

            start.unit = this;
            //If leaving old cell changed invaded status
            if (start.country && start.country.isInvaded)
            {
                start.unit = null;
                if (!start.country.isInvaded)
                {
                    start.country.TriggerLiberation();
                }
            }

            if (startUnit)
            {
                start.unit = startUnit;
            }
            else
            {
                start.unit = null;
            }
            if (destUnit)
            {
                destination.unit = destUnit;
            }
            else
            {
                destination.unit = null;
            }
        }
    }

    /// <summary>
    /// Unit shoots at specified cell
    /// If unit on said cell looses enough health it's destroyed
    /// </summary>
    /// <param name="cell"></param>
    public virtual void ShootAt(LogicalMapCell cell)
    {
        hasAttacked = true;
        cell.unit.healthPoints -= type.attackPower;
        cell.unit.DisplayHealth();
        if (cell.unit.healthPoints <= 0)
        {
            cell.unit.DestroyVisually();
        }
        movePoints = 0;
        DisplayMP();
        DisplayAP();
    }

    /// <summary>
    /// Removes unit from game
    /// </summary>
    public virtual void DestroyLogically()
    {
        GameMaster.units.Remove(this);
        cell.unit = null;
        Destroy(gameObject);
    }

    /// <summary>
    /// Changes unit's model to destroyed one and marks it for logical destraction
    /// </summary>
    public virtual void DestroyVisually()
    {
        if (cell.country && cell.country.isInvaded)
        {
            isDestroyed = true;
            if (!cell.country.isInvaded)
            {
                cell.country.TriggerLiberation();
            }
        }
        isDestroyed = true;
        GetComponentInChildren<MeshRenderer>().material.color = Color.black;        
    }

    /// <summary>
    /// Aligns unit's position with relative phisical map terrain
    /// </summary>
    public void ValidatePosition()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, new Vector3(0f, -1f, 0f), out hit);

        Vector3 position = hit.point;
        position.y += 0.5f;

        this.transform.position = position;
        this.transform.localRotation = Quaternion.Euler(hit.normal);
    }

    /// <summary>
    /// Method for embarking transport
    /// </summary>
    /// <param name="cell"></param>
    public virtual void Embark(LogicalMapCell cell)
    {
        transport = (Transport) cell.unit;

        List<LogicalMapCell> path = Pathfinder.SearchPath(this.cell, cell);
        AffectCountryes(path);

        movePoints -= cell.distance;
        this.cell.unit = null;
        this.cell = cell;
        ShowEmbarkCanvas();
        StopAllCoroutines();
        StartCoroutine(Travel(path));

        transport.TakeAboard(this);
    }

    /// <summary>
    /// Method for disembarking transport
    /// </summary>
    /// <param name="cell"></param>
    public virtual void Disembark(LogicalMapCell cell)
    {
        List<LogicalMapCell> path = Pathfinder.SearchPath(this.cell, cell);
        AffectCountryes(path);

        movePoints -= cell.distance;
        this.cell = cell;
        cell.unit = this;

        ShowDisembarkCanvas();

        GetComponentInChildren<MeshRenderer>().enabled = true;
        StopAllCoroutines();
        StartCoroutine(Travel(path));       
        transport.RemoveFromTransport(this);
        transport = null;
    }
    void ShowEmbarkCanvas()
    {
        if (!platform)
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            HideCanvas();

            if (type.attackPower > 0)
            {
                transform.Find("UnitCanvas(Clone)").Find("Icon1").gameObject.SetActive(true);

                Vector3 pos2 = transform.Find("UnitCanvas(Clone)").Find("Icon3").localPosition;
                pos2.x = 6.65f;
                transform.Find("UnitCanvas(Clone)").Find("Icon3").localPosition = pos2;

                Vector3 pos = transform.Find("UnitCanvas(Clone)").Find("Icon1").localPosition;
                pos.x = -6.42f;
                transform.Find("UnitCanvas(Clone)").Find("Icon1").localPosition = pos;

                transform.Find("UnitCanvas(Clone)").Find("Icon3").gameObject.SetActive(true);

                if (this is SuperUnit) transform.GetChild(1).Find("Icon5").gameObject.SetActive(true);
            }
        }

    }

    void ShowDisembarkCanvas()
    {
        DisplayMP();
        if (!platform)
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            ShowCanvas();
            if (type.attackPower > 0)
            {
                Vector3 pos = transform.Find("UnitCanvas(Clone)").Find("Icon1").localPosition;
                pos.x = -5.74f;
                transform.Find("UnitCanvas(Clone)").Find("Icon1").localPosition = pos;

                Vector3 pos2 = transform.Find("UnitCanvas(Clone)").Find("Icon3").localPosition;
                pos2.x = 3.74f;
                transform.Find("UnitCanvas(Clone)").Find("Icon3").localPosition = pos2;
            }
        }

    }

    /// <summary>
    /// Method for boarding platform
    /// </summary>
    /// <param name="cell"></param>
    public void BoardPlatform(LogicalMapCell cell)
    {
        platform = (Platform)cell.unit;
        platform.boardedUnit = this;
        platform.DisplayEP();
        List<LogicalMapCell> path = Pathfinder.SearchPath(this.cell, cell);
        AffectCountryes(path);

        movePoints -= cell.distance;
        this.cell.unit = null;
        this.cell = cell;

        StopAllCoroutines();
        StartCoroutine(Travel(path));

        ShowEmbarkCanvas();
    }



    /// <summary>
    /// Method for leaving platform
    /// </summary>
    /// <param name="cell"></param>
    public void LeavePlatform(LogicalMapCell cell)
    {
        movePoints -= cell.distance;
        ShowDisembarkCanvas();
        platform.boardedUnit = null;
        platform.DisplayEP();
        platform = null;

        List<LogicalMapCell> path = Pathfinder.SearchPath(this.cell, cell);
        AffectCountryes(path);
        this.cell = cell;
        cell.unit = this;


        StopAllCoroutines();
        StartCoroutine(Travel(path));
    }
}
