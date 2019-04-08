using SPNCipher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DifferentialCyrptanalysis {
    public class DifferentialAnalyser {
        private const double LOWER_BOUND_PROBABILITY = 0.01;
        private readonly ISPNCipher _spn;

        private PlainTextGenerator _generator;
        private List<DifferentialCharacteristic> _characteristics;
        private ISubstitution Substitution { get { return _spn.Substitution; } }
        private IPermutation Permutation { get { return _spn.Permutation; } }

        public DistributionTable Table { get; }
        public DifferentialCharacteristic Characteristic { get; private set; }
        public List<int> ActiveSubstitutionBoxesInLastRound { get; private set; }

        public DifferentialAnalyser(ISPNCipher spn) {
            _spn = spn;
            _generator = new PlainTextGenerator(_spn.BlockLength);
            Table = new DistributionTable(spn.Substitution);
            _characteristics = new List<DifferentialCharacteristic>();
        }

        #region Characteristic Creation
        public DifferentialCharacteristic CreateCharacteristic() {
            List<int> inputDifferences = GetPossibleInputs();
            foreach (int inputDifference in inputDifferences) {
                List<DifferentialCharacteristic> characteristics = GetCharacteristicsForSPN(inputDifference);

                _characteristics.AddRange(characteristics);
            }

            _characteristics = _characteristics.OrderByDescending(e => e.Probability).ToList();
            Characteristic = _characteristics.First();

            ActiveSubstitutionBoxesInLastRound = FindActiveSubstitutionBoxes(Characteristic.OutputDifference);

            return Characteristic;
        }

        private List<int> GetPossibleInputs() {
            List<int> inputs = new List<int>();
            int maxInput = (int)Math.Pow(2, Substitution.InputLength) - 1;

            for (int i = 0; i < _spn.SubstitutionBoxCount; i++) {
                int shiftAmount = (i * Substitution.InputLength);
                for (int j = 1; j <= maxInput; j++) {
                    inputs.Add(j << shiftAmount);
                }
            }

            return inputs;
        }

        private List<DifferentialCharacteristic> GetCharacteristicsForSPN(int inputDifference) {
            List<DifferentialCharacteristic> inputCharacteristics = new List<DifferentialCharacteristic>() { new DifferentialCharacteristic(inputDifference, inputDifference, 1) };
            for (int r = 1; r < _spn.RoundCount; r++) {
                for (int i = inputCharacteristics.Count - 1; i >= 0; i--) {
                    DifferentialCharacteristic inputCharacteristic = inputCharacteristics[i];
                    inputCharacteristics.RemoveAt(i);

                    List<DifferentialCharacteristic> outputCharacteristics = GetCharacteristicForRound(inputCharacteristic);

                    inputCharacteristics.AddRange(outputCharacteristics);
                }
            }

            return inputCharacteristics;
        }

        private List<DifferentialCharacteristic> GetCharacteristicForRound(DifferentialCharacteristic characteristic) {
            byte[] sBoxInputs = GetBytes(characteristic.OutputDifference);
            Dictionary<int, List<SubstitutionBoxOutput>> sBoxOutputs = GetSubstitutionOutputs(sBoxInputs);

            List<DifferentialCharacteristic> possibleOutcomes = GetPossibleSubstitutionOutcomes(characteristic.InputDifference, sBoxOutputs, characteristic.Probability);
            possibleOutcomes.ForEach(e => {
                e.AddIntermediateOutputs(characteristic.IntermediateOutputs);
                e.AddIntermediateOutput(characteristic.OutputDifference);

                List<List<SubstitutionBoxOutput>> usedBoxes = new List<List<SubstitutionBoxOutput>>(characteristic.UsedSubstitutionBoxes);
                usedBoxes.AddRange(e.UsedSubstitutionBoxes);
                e.SetUsedSubstitionBoxes(usedBoxes);
            });

            List<DifferentialCharacteristic> permutatedOutcomes = GetPermutatedOutcomes(possibleOutcomes);

            return permutatedOutcomes;
        }

        private Dictionary<int, List<SubstitutionBoxOutput>> GetSubstitutionOutputs(byte[] inputs) {
            Dictionary<int, List<SubstitutionBoxOutput>> subBoxOutputs = new Dictionary<int, List<SubstitutionBoxOutput>>();

            int sBox = 0;
            foreach (byte sBoxInput in inputs) {
                if (sBoxInput != 0) {
                    List<SubstitutionBoxOutput> boxOutputs = GetSubstitutionOutputs(sBoxInput);
                    subBoxOutputs.Add(sBox, boxOutputs);
                }

                sBox++;
            }

            return subBoxOutputs;
        }

        private List<SubstitutionBoxOutput> GetSubstitutionOutputs(byte input) {
            List<SubstitutionBoxOutput> outputs = new List<SubstitutionBoxOutput>();

            Dictionary<byte, byte> differenceTable = Table[input];
            double divisor = Math.Pow(Substitution.InputLength, 2);
            foreach (KeyValuePair<byte, byte> difference in differenceTable) {
                if (difference.Value > 0)
                    outputs.Add(new SubstitutionBoxOutput(input, difference.Key, difference.Value / divisor));
            }

            return outputs.OrderByDescending(o => o.Score).ToList();
        }

        private List<DifferentialCharacteristic> GetPossibleSubstitutionOutcomes(int inputDifference, Dictionary<int, List<SubstitutionBoxOutput>> sBoxOutputs, double previosProbability = 1) {
            List<List<SubstitutionBoxOutput>> allOutputs = sBoxOutputs.Select(k => k.Value).ToList();
            IEnumerable<IEnumerable<SubstitutionBoxOutput>> possibleOutcomes = allOutputs.CartesianProduct();

            List<DifferentialCharacteristic> outcomes = new List<DifferentialCharacteristic>();
            foreach (IEnumerable<SubstitutionBoxOutput> possibleOutcome in possibleOutcomes) {
                List<int> sBoxIndexes = sBoxOutputs.Keys.ToList();
                int currentIndex = 0, output = 0;
                double probability = previosProbability;
                foreach (SubstitutionBoxOutput sBoxOutput in possibleOutcome) {
                    output |= (sBoxOutput.Output << ((_spn.SubstitutionBoxCount - 1 - sBoxIndexes[currentIndex++]) * Substitution.InputLength));
                    probability *= sBoxOutput.Bias;
                }

                if (probability > LOWER_BOUND_PROBABILITY) {
                    DifferentialCharacteristic characteristic = new DifferentialCharacteristic(inputDifference, output, probability);
                    characteristic.AddUsedSubstitionBoxForRound(possibleOutcome.ToList());

                    outcomes.Add(characteristic);
                }
            }

            return outcomes;
        }

        private List<DifferentialCharacteristic> GetPermutatedOutcomes(List<DifferentialCharacteristic> characteristics) {
            List<DifferentialCharacteristic> permutatedOutcomes = new List<DifferentialCharacteristic>();
            foreach (DifferentialCharacteristic characteristic in characteristics) {
                int output = Permutation.Permutate(characteristic.OutputDifference);
                DifferentialCharacteristic newCharacteristic = new DifferentialCharacteristic(characteristic.InputDifference, output, characteristic.Probability);
                newCharacteristic.AddIntermediateOutputs(characteristic.IntermediateOutputs);
                newCharacteristic.AddUsedSubstitionBoxes(characteristic.UsedSubstitutionBoxes);

                permutatedOutcomes.Add(newCharacteristic);
            }

            return permutatedOutcomes;
        }

        private List<int> FindActiveSubstitutionBoxes(int output) {
            List<int> activeBoxes = new List<int>();

            int substitutionMask = (1 << Substitution.InputLength) - 1;
            for (int i = 0; i < _spn.SubstitutionBoxCount; i++) {
                if (((output >> (i * Substitution.InputLength)) & substitutionMask) != 0) {
                    activeBoxes.Add(_spn.SubstitutionBoxCount - i - 1);
                }
            }

            activeBoxes = activeBoxes.OrderBy(a => a).ToList();
            return activeBoxes;
        }
        #endregion

        #region Cryptanalysis
        public int Analyse() {
            if (_characteristics == null || _characteristics.Count == 0 || Characteristic == null)
                CreateCharacteristic();

            int neededPairs = (int)Math.Ceiling(20f / Characteristic.Probability);

            Dictionary<int, int> plainTexts = _generator.GetPlainTexts(neededPairs, Characteristic.InputDifference);
            List<int> partialKeys = GeneratePartialKeys();

            Dictionary<int, double> partialKeyMatchProbabilities = CalculateOutputDifference(plainTexts, partialKeys);
            return partialKeyMatchProbabilities.First().Key;
        }

        private Dictionary<int, double> CalculateOutputDifference(Dictionary<int, int> inputs, List<int> partialKeys) {
            Dictionary<int, double> partialKeyMatchProbabilities = new Dictionary<int, double>();
            byte[] inputsOfLastSubBoxes = GetBytes(Characteristic.OutputDifference);

            double correctPairs = 0;
            foreach (KeyValuePair<int, int> inputPair in inputs) {
                int firstOutput = _spn.Encode(inputPair.Key);
                int secondOutput = _spn.Encode(inputPair.Value);
                int outputDifference = firstOutput ^ secondOutput;
                byte[] outputsOfDifference = GetBytes(outputDifference);

                bool invalidPair = false;
                for (int i = 0; i < _spn.SubstitutionBoxCount; i++) {
                    if (!ActiveSubstitutionBoxesInLastRound.Contains(i)) {
                        if (outputsOfDifference[i] != 0) {
                            invalidPair = true;
                            break;
                        }
                    }
                }
                if (invalidPair)
                    continue;

                correctPairs++;
                foreach (int partialKey in partialKeys) {
                    int firstOutputOfLastSubBoxes = (firstOutput ^ partialKey);
                    int secondOutputOfLastSubBoxes = (secondOutput ^ partialKey);
                    byte[] firstOutputsOfLastSubBoxes = GetBytes(firstOutputOfLastSubBoxes);
                    byte[] secondOutputsOfLastSubBoxes = GetBytes(secondOutputOfLastSubBoxes);

                    bool partialKeyMatched = true;
                    for (int i = 0; i < _spn.SubstitutionBoxCount; i++) {
                        if (ActiveSubstitutionBoxesInLastRound.Contains(i)) {
                            firstOutputsOfLastSubBoxes[i] = Substitution.InverseSubstitute(firstOutputsOfLastSubBoxes[i]);
                            secondOutputsOfLastSubBoxes[i] = Substitution.InverseSubstitute(secondOutputsOfLastSubBoxes[i]);
                            byte difference = (byte)(firstOutputsOfLastSubBoxes[i] ^ secondOutputsOfLastSubBoxes[i]);

                            if (difference != inputsOfLastSubBoxes[i]) {
                                partialKeyMatched = false;
                                break;
                            }
                        }
                    }

                    if (partialKeyMatched) {
                        if (!partialKeyMatchProbabilities.ContainsKey(partialKey))
                            partialKeyMatchProbabilities.Add(partialKey, 0);
                        partialKeyMatchProbabilities[partialKey]++;
                    }
                }
            }

            partialKeyMatchProbabilities = partialKeyMatchProbabilities.OrderByDescending(p => p.Value).ToDictionary(k => k.Key, v => v.Value / correctPairs);
            return partialKeyMatchProbabilities;
        }

        private List<int> GeneratePartialKeys() {
            List<int> keys = new List<int>();

            int substitutionMask = (1 << Substitution.InputLength) - 1;
            int maxValue = (int)Math.Pow(2, ActiveSubstitutionBoxesInLastRound.Count * 4);
            for (int i = 0; i < maxValue; i++) {
                int key = 0;
                for (int j = 0; j < ActiveSubstitutionBoxesInLastRound.Count; j++) {
                    key |= (i >> (j * Substitution.InputLength) & substitutionMask) << ((_spn.SubstitutionBoxCount - 1 - ActiveSubstitutionBoxesInLastRound[j]) * Substitution.InputLength);
                }
                keys.Add(key);
            }

            return keys;
        }
        #endregion

        private byte[] GetBytes(int input) {
            byte[] outputs = new byte[_spn.SubstitutionBoxCount];
            byte mask = (byte)((1 << Substitution.InputLength) - 1);
            for (int i = 0; i < _spn.SubstitutionBoxCount; i++) {
                outputs[i] = (byte)(input >> ((_spn.SubstitutionBoxCount - 1 - i) * Substitution.InputLength) & mask);
            }

            return outputs;
        }

        private int GetInt(byte[] inputBytes) {
            int output = 0;
            for (int i = 0; i < inputBytes.Length; i++) {
                output |= inputBytes[i] << ((_spn.SubstitutionBoxCount - i - 1) * Substitution.InputLength);
            }

            return output;
        }
    }

    public class DifferentialCharacteristic {
        public int InputDifference { get; }
        public int OutputDifference { get; }
        public double Probability { get; }
        public List<int> IntermediateOutputs { get; }
        public List<List<SubstitutionBoxOutput>> UsedSubstitutionBoxes { get; }

        public DifferentialCharacteristic(int inputDifference, int outputDifference, double probability) {
            InputDifference = inputDifference;
            OutputDifference = outputDifference;
            Probability = probability;

            IntermediateOutputs = new List<int>();
            UsedSubstitutionBoxes = new List<List<SubstitutionBoxOutput>>();
        }

        internal void AddIntermediateOutput(int output) {
            IntermediateOutputs.Add(output);
        }

        internal void AddIntermediateOutputs(IEnumerable<int> outputs) {
            IntermediateOutputs.AddRange(outputs);
        }

        internal void AddUsedSubstitionBoxForRound(List<SubstitutionBoxOutput> boxes) {
            UsedSubstitutionBoxes.Add(boxes);
        }

        internal void AddUsedSubstitionBoxes(List<List<SubstitutionBoxOutput>> boxes) {
            UsedSubstitutionBoxes.AddRange(boxes);
        }

        internal void SetUsedSubstitionBoxes(List<List<SubstitutionBoxOutput>> boxes) {
            UsedSubstitutionBoxes.Clear();
            UsedSubstitutionBoxes.AddRange(boxes);
        }

        public override string ToString() {
            return $"{InputDifference} X {OutputDifference} ==> {Probability}";
        }
    }

    public class SubstitutionBoxOutput {
        public byte Input { get; }
        public byte Output { get; }
        public double Bias { get; }
        public double Score { get; }

        public SubstitutionBoxOutput(byte input, byte output, double bias) {
            Input = input;
            Output = output;
            Bias = bias;
            Score = Bias / GetNumberOfOnes();
        }

        private int GetNumberOfOnes() {
            int ones = 0;
            for (int i = 0; i < 8; i++) {
                if (((Output >> i) & 1) == 1)
                    ones++;
            }

            return ones;
        }

        public override string ToString() {
            return $"Input: {Input.ToString("X")} - Output: {Output.ToString("X")} - Bias: {Bias} - Score: {Score.ToString("0.###")}";
        }
    }
}
