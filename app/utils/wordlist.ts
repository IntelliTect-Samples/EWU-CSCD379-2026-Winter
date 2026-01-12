export const WORD_LIST = [
  { word: 'apple', hint: 'A round fruit that can be red, green, or yellow and is picked around fall.' },
  { word: 'bread', hint: 'A baked good that can be made at home or bought.' },
  { word: 'cloud', hint: 'Water vapor that floats in the sky.' },
  { word: 'brain', hint: 'You use this to solve puzzles.' },
  { word: 'earth', hint: 'The planet we live on and another name for dirt.' },
  { word: 'flame', hint: 'A small one can be made from a lighter.' },
  { word: 'grape', hint: 'A small, round purple or green fruit that grows on vines.' },
  { word: 'house', hint: 'A building that serves as a living space.' },
  { word: 'juice', hint: 'The liquid squeezed from fruit or vegetables.' },
  { word: 'light', hint: 'The sun is a source of this.' },
  { word: 'ocean', hint: 'A very large expanse of sea.' },
  { word: 'piano', hint: 'An instrument with black and white keys.' },
  { word: 'goose', hint: 'Is loud, sometimes aggressive, and flies. Loves bread.' },
  { word: 'river', hint: 'A natural stream of water.' },
  { word: 'sugar', hint: 'Sweet crystals obtained from cane or beet.' },
  { word: 'train', hint: 'A series of connected railway carriages or wagons.' },
  { word: 'cocoa', hint: 'Chocolate drink for when the weather gets cold.' },
  { word: 'water', hint: 'A transparent liquid that makes up seas and lakes.' },
  { word: 'mummy', hint: 'Tutankhamun is one of these.' },
  { word: 'zebra', hint: 'A horse with black-and-white stripes.' }
];

export const VALID_GUESSES = [
  'stare', 'crane', 'slate', 'brave', 'plane', 'blame', 'glare', 'pride', 'grime', 'route',
  'shine', 'charm', 'flair', 'glide', 'brisk', 'climb', 'sling', 'trice', 'whale', 'zesty',
  'quilt', 'vigor', 'waltz', 'xenon', 'yacht', 'zonal', 'adobe', 'baker', 'cider', 'drape',
  'eagle', 'fable', 'gamer', 'hiker', 'ivory', 'jolly', 'kneel', 'lunar', 'mango', 'noble',
  'oasis', 'quark', 'raven', 'salad', 'tiger', 'ultra', 'vivid', 'woven', 'yodel', 'abide', 
  'bloom', 'chase', 'daisy', 'eager', 'frost', 'grace', 'haste', 'image', 'jewel', 'lemon', 
  'novel', 'opera', 'quest', 'rally', 'sugar', 'tulip', 'unity', 'vapor', 'witty', 'count',
  'adieu', 'audio', 'raise', 'least', 'train', 'sleep', 'power', 'cloud', 'music', 'field', 
  'night', 'heart', 'light', 'point', 'earth', 'bread', 'table', 'smile', 'water', 'plant',
  'apple', 'grape', 'house', 'piano', 'ocean', 'flame', 'brain', 'juice', 'cocoa', 'river',
  'mummy', 'zebra', 'goose', 'fruit'
];

export const getRandomWord = () => {
  return WORD_LIST[Math.floor(Math.random() * WORD_LIST.length)]
}