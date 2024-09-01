namespace CleanSweep.Interfaces
{
    using System.Threading.Tasks;

    public interface ICleaner
    {
        (string FileType, int SpaceInMB) GetReclaimableSpace();
        Task Reclaim();
    }
}
