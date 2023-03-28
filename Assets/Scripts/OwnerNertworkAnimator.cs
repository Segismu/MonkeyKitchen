using Unity.Netcode.Components;

public class OwnerNertworkAnimator : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
