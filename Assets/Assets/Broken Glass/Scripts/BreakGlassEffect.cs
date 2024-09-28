using UnityEngine;
using UnityEngine.Events;

public class BreakGlassEffect : Singleton<BreakGlassEffect> {

	private Material _glassMat;
    private float MaxDistortion = 100;
    public UnityEvent onVehicleDestroyed;

    private float _distortion;
    private float Distortion
    {
        get => _distortion;
        set => _distortion = Mathf.Clamp(value, 0, MaxDistortion);
    }

    void Start () => _glassMat = GetComponent<MeshRenderer>().material;

    void Update () => _glassMat.SetFloat("_BumpAmt", _distortion);

	public void BreakForce(float force) => Distortion += force * 2;

}
