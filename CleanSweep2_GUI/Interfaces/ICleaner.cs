namespace CleanSweep2.Interfaces
{
    public interface ICleaner
    {
        (string FileType, int SpaceInMB) GetReclaimableSpace();
    }
}
