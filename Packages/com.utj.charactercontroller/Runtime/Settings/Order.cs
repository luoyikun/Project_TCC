
namespace Unity.TinyCharacterController.Utility
{
    public static class Order
    {
        public const int GameObjectPool = -1000;
        public const int IndicatorRegister = -100;
        
        // ----------------------------------
        // early update
        // ----------------------------------

        // Prepare Process
        public const int PrepareEarlyUpdate = -1000;
        
        // ----------------------------------
        // update
        // ----------------------------------

        public const int Check = -200;
        public const int Gravity = -100;
        // 自己完結
        public const int Effect = -50;
        // Control
        public const int Control = 5;
        
        // ----------------------------------
        // 应用计算结果（ExecuteOrder）。
        // ----------------------------------

        public const int EarlyUpdateBrain = -101;   // 在 Input System 更新之前。
        public const int UpdateBrain = 10;          // Brain 的更新时间。
        public const int PostUpdate = 100;          // 在 Brain 更新之后。
        public const int UpdateIK = 50;             // IK 的更新时间。
    }
}
