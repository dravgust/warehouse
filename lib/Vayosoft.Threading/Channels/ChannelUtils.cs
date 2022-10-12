using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Vayosoft.Threading.Channels
{
    public static class ChannelUtils
    {
        public static ChannelReader<T> Merge<T>(params ChannelReader<T>[] inputs)
        {
            var output = Channel.CreateUnbounded<T>();

            Task.Run(async () =>
            {
                async Task Redirect(ChannelReader<T> input)
                {
                    await foreach (var item in input.ReadAllAsync())
                        await output.Writer.WriteAsync(item);
                }

                await Task.WhenAll(inputs.Select(Redirect).ToArray());
                output.Writer.Complete();
            });

            return output;
        }

        public static IList<ChannelReader<T>> Split<T>(ChannelReader<T> ch, int n)
        {
            var outputs = new Channel<T>[n];

            for (var i = 0; i < n; i++)
                outputs[i] = Channel.CreateUnbounded<T>();

            Task.Run(async () =>
            {
                var index = 0;
                await foreach (var item in ch.ReadAllAsync())
                {
                    await outputs[index].Writer.WriteAsync(item);
                    index = (index + 1) % n;
                }

                foreach (var c in outputs)
                    c.Writer.Complete();
            });

            return outputs.Select(c => c.Reader).ToArray();
        }
    }
}
