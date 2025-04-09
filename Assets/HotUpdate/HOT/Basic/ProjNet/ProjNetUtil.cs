using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet.SRID;
using UnityEngine;

public class ProjNetUtil : ASingleton<ProjNetUtil>
{
    public double offsetX;
    public double offsetY;
    public double offsetZ;
    private CoordinateSystem projCs;
    private CoordinateSystem geoCs;
    private CoordinateTransformationFactory ctFactory;
    private ICoordinateTransformation transformation;
    private ICoordinateTransformation ReTransformation;

    private (double,double,double) position;

    public (double, double, double) Position
    {
        get {
            return new (offsetX, offsetY, offsetZ);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        projCs = SRIDReader.GetCSbyID(4326);
        geoCs = SRIDReader.GetCSbyID(4546);
        ctFactory = new CoordinateTransformationFactory();
        transformation = ctFactory.CreateFromCoordinateSystems(projCs, geoCs);
        ReTransformation = ctFactory.CreateFromCoordinateSystems(geoCs,projCs);
    }

    public Vector3 Transform(double[] point)
    {
        double[] pos = transformation.MathTransform.Transform(point);
        pos[0] -= offsetX;
        pos[1] -= offsetZ;
        return new Vector3((float)pos[0], (float)offsetY, (float)pos[1]);
    }

    public (double x, double z) Reverse(double x,double z) {
        x += offsetX;
        z += offsetZ;
        return ReTransformation.MathTransform.Transform(x, z);
    }
}
