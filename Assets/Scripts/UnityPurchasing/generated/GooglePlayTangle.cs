// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("ZIarrfgyIEy7ZxWw6VqgQ6vGsuqPTQBsl2z4+f+vooQyIfSmc0wHLy49s6THF3d0nw3a0QjAWYS/DviSXXNe3bQ3HGbfCXeKkK6+Ax+NeP+voHFqjZzh1Gla8uorYKcywxOfvjS3ubaGNLe8tDS3t7YpHJML58EN8bKgoOQOX8+fYd75u0F7a110GrGGNLeUhruwv5ww/jBBu7e3t7O2tfM92fSNrzcV/o2crqtMSYpEzJEwybhFtvyqp9YQ2Xz+BM5RP+jz8AiC0/fuzXFM7e1VoQNh6KroM2BN15ftUwpbdA1fibpisAVUzeyFIv8KHaH2dka0gqlratOhX1UjSnwDYxeB6pNXrkI6hPYp7xnd+2RQrzQjozgGvqk/hAWpX7S1t7a3");
        private static int[] order = new int[] { 8,9,13,11,7,9,6,8,10,12,12,11,13,13,14 };
        private static int key = 182;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
