using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sheesh
{
    public class Car
{
    public int Id { get; private set; }
    private string state;
    private int taskEndTime;
    private int currentResource;
    public string CompletedTask { get; set; }
    public int LastResource => currentResource;

    public Car(int id)
    {
        Id = id;
        this.state = "Idle";
    }

    public void TransportToFactory1(int resource, int startTime, int processTime)
    {
        this.currentResource = resource;
        this.state = "Transporting to Factory 1";
        this.taskEndTime = startTime + processTime;
        this.CompletedTask = null;
    }

    public void TransportToFactory2(int resource, int startTime, int processTime)
    {
        this.currentResource = resource;
        this.state = "Transporting to Factory 2";
        this.taskEndTime = startTime + processTime;
        this.CompletedTask = null;
    }

    public void DeliverFinalProduct(int resource, int startTime)
    {
        this.currentResource = resource;
        this.state = "Delivering final product";
        this.taskEndTime = startTime + 1; // Assuming delivery time is negligible
        this.CompletedTask = null;
    }

    public void Update(int currentTime)
    {
        if (currentTime >= taskEndTime && state != "Idle")
        {
            CompletedTask = state.Contains("Factory 1") ? "Factory1" :
                            state.Contains("Factory 2") ? "Factory2" : "FinalSpot";
            state = "Idle";
        }
    }

    public bool IsIdle()
    {
        return state == "Idle";
    }
}
}
